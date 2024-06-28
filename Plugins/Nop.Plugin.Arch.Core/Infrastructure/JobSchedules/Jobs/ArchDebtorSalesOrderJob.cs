using ArchServiceReference;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Arch.Core.Services.Orders;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules.Jobs
{
    /// </summary>
    public class ArchDebtorsSalesOrderListJob : ArchScheduledJobBase<GetDebtorsSalesOrderResponse.DebtorsSalesOrderElement>
    {
        protected override Type TaskType => typeof(ArchDebtorsSalesOrderListJob);

        private const string StartDateSettingParam = "ArchDebtorsSalesOrderListTask_StartDate";

        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IShipmentService _shipmentService;
        private readonly IArchInvoiceService _invoiceService;
        private readonly IResanehlabService _resanehlabService;

        public ArchDebtorsSalesOrderListJob(IOrderService orderService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ISettingService settingService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            ILogger logger,
            IObjectConverter objectConverter,
            IEventPublisher eventPublisher,
            IOrderProcessingService orderProcessingService,
            IShipmentService shipmentService,
            ILogger<ScheduledJobBase<object>> jobLogger)
            : base(settingService,
                storeService,
                storeContext,
                storeMappingService,
                urlRecordService,
                logger,
                objectConverter,
                jobLogger)
        {
            _orderService = orderService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _eventPublisher = eventPublisher;
            _orderProcessingService = orderProcessingService;
            _shipmentService = shipmentService;
            _invoiceService = EngineContext.Current.Resolve<IArchInvoiceService>();
            _resanehlabService = EngineContext.Current.Resolve<IResanehlabService>();
        }

        protected override void Produce()
        {
            var now = DateTime.Now;
            var startDate = GetLastUpdate(StartDateSettingParam);
            var endDate = new DateTime(now.Year, now.Month, now.Day).AddDays(1).AddTicks(-1);

            Debug($"Fetching Orders");

            // only processing paid orders in pending or processing state
            var orderStatusIds = new List<OrderStatus> { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Picked }.Select(i => (int)i).ToList();
            var paymentStatusIds = new List<PaymentStatus> { PaymentStatus.Paid }.Select(i => (int)i).ToList();

            var orders = _orderService.SearchOrders(RunningOnStoreId, osIds: orderStatusIds, psIds: paymentStatusIds);
            foreach (var order in orders)
            {
                var customer = _customerService.GetCustomerById(order.CustomerId);
                if (customer == null)
                    continue;

                var debtorNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.DebtorNumberAttribute, RunningOnStoreId);
                if (string.IsNullOrEmpty(debtorNumber))
                {
                    debtorNumber = _archSettings.DefaultDebtorCode;
                }

                Debug($"Calling ArchAPI Order: {order.Id} Tracking Number: {order.TransactionTrackingNumber}");
                var response = ArchClient.GetDebtorsSalesOrderListAsync(new GetDebtorsSalesOrderRequest
                {
                    SystemAuthenticationCode = _archSettings.SystemAuthenticationCode,
                    TransactionTrackingNumber = order.TransactionTrackingNumber,
                    DebtorNumber = debtorNumber,
                    StartDate = order.CreatedOnUtc.Date,
                    EndDate = endDate
                }).ConfigureAwait(false).GetAwaiter().GetResult();

                var debtorsSalesOrderElements = response.List;
                if (!response.List.Any() && order.OrderStatus == OrderStatus.Pending && order.PaymentStatus == PaymentStatus.Paid && order.TransactionTrackingNumber == 0M)// Arch doesn't have the Sales Order
                {
                    var deliveryDateTime = _resanehlabService.GetEstimatedDateForOrder(order.Id);
                    _invoiceService.ConfirmArch(order, customer, order.StoreId, deliveryDateTime);
                    Debug($"API returning no result for: {order.Id} Tracking Number: {order.OrderTrackingNumber}");
                }
                foreach (var debtorsSalesOrderElement in debtorsSalesOrderElements)
                {
                    EnqueueItem(debtorsSalesOrderElement);
                }
            }
            Debug($"Completed Producing");

            SetLastUpdate(StartDateSettingParam);
        }

        protected override void Consume(GetDebtorsSalesOrderResponse.DebtorsSalesOrderElement item)
        {
            var order = _orderService.GetOrderFromOrderAndTrackingNumber(item.OrderNumber, item.TransactionTrackingNumber, storeId: RunningOnStoreId);

            if (order == null)
            {
                order = _orderService.GetOrderByTransactionTrackingNumber(item.TransactionTrackingNumber, storeId: RunningOnStoreId);
            }

            if (order == null)
            {
                Debug($"Could not find order with tracking number {item.TransactionTrackingNumber}");
                return;
            }

            if (order.PaymentStatus == PaymentStatus.Failed)
                return;

            Debug($"Consuming {order.Id} with Status {item.Status}");

            var customer = _customerService.GetCustomerById(order.CustomerId);

            switch (item.Status)
            {
                case (int)DebtorSalesOrderStatusType.Quotation:
                    break;

                case (int)DebtorSalesOrderStatusType.Pending:
                    break;

                case (int)DebtorSalesOrderStatusType.Picked:
                    _orderProcessingService.MarkOrderAsPicked(order);
                    break;

                case (int)DebtorSalesOrderStatusType.Dispatched:
                    if (order.ShippingStatus == ShippingStatus.NotYetShipped)
                    {
                        // Arch validation process will not be asked for in terms of a customer who is a debtor with arch
                        // i.e., not a cash debtor not an anonymous customer.
                        // We need to ensure that the OTP is still sent out under this condition. 
                        if (customer.IsDebtor(RunningOnStoreId))
                        {
                            // send OTP to customer for the now validated order
                            _eventPublisher.Publish(new ArchValidateEvent(order));
                        }

                        _orderProcessingService.UpdateToReadyForCollection(order, shipment: null);
                    }
                    break;

                case (int)DebtorSalesOrderStatusType.Cancelled:

                    if (_orderProcessingService.CanCancelOrder(order))
                        _orderProcessingService.CancelOrder(order, notifyCustomer: true, "Arch User");
                    else
                        Debug($"Cannot cancel order {order.Id}");

                    break;
            }

            Debug($"Completed processing {order.Id}");
        }

        protected override void CollectData() { }
    }
}
