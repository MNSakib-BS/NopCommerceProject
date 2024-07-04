using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Models.Arch;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Plugin.Arch.Core.Domains.Payments;
using Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
using Nop.Plugin.Arch.Core.Services.Payments;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;

public class ArchService : IArchService
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly ICustomerService _customerService;
    private readonly IWorkflowMessageService _workflowMessageService;
    private readonly LocalizationSettings _localizationSettings;
    private readonly IPriceFormatter _priceFormatter;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEncryptionService _encryptionService;
    private readonly ICustomerWalletService _customerWalletService;
    private readonly IOrderProcessingService _orderProcessingService;
    private ArchApiBase _archApiBase;

    public ArchService(ICustomerService customerService,
        IOrderService orderService,
        IPaymentService paymentService,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        IPriceFormatter priceFormatter,
        IEventPublisher eventPublisher,
        IEncryptionService encryptionService,
        ICustomerWalletService customerWalletService,
        IOrderProcessingService orderProcessingService)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _customerService = customerService;
        _priceFormatter = priceFormatter;
        _eventPublisher = eventPublisher;
        _encryptionService = encryptionService;
        _customerWalletService = customerWalletService;
        _orderProcessingService = orderProcessingService;

    }

    public async Task<string> ComputePayloadChecksumAsync(ArchRefundRequest request)
    {
        var preHashedCode = $"{request.authentication_code}{request.refund_type}{request.total}{request.transaction_code}";
        var hash = await _encryptionService.ComputeSha256HashAsync(preHashedCode);
        return hash;
    }

    private ArchRefundResponse CreateResponse(KeyValuePair<int, string> responseKvp)
    {
        var refundResponse = new ArchRefundResponse
        {
            status = responseKvp.Key,
            message = responseKvp.Value
        };

        return refundResponse;
    }

    public async Task<ArchRefundResponse> RefundAsync(ArchRefundRequest request, Func<ArchRefundRequest, bool> payloadChecksumValidation)
    {
        if (!payloadChecksumValidation.Invoke(request))
            return CreateResponse(ArchRefundStatusMessage.UnableToCapture);

        if (request.total < 0)
            return CreateResponse(ArchRefundStatusMessage.RefundAmountLessThanZero);

        var order = await _orderService.GetOrderByTransactionTrackingNumber(request.transaction_code, storeId: request.storeid);
        if (order == null)
            return CreateResponse(ArchRefundStatusMessage.OrderDoesNotExist);

        var refundCaptured = await _customerWalletService.IsRefundCapturedAsync(order.CustomerId, order.StoreId, request.refund_type, request.transaction_code);
        if (refundCaptured)
            return CreateResponse(ArchRefundStatusMessage.RefundAlreadyCaptured);

        var paidOnAccount = order.PaidOnAccount.HasValue && order.PaidOnAccount.Value > 0 ? order.PaidOnAccount.Value : 0;
        var paidWithWallet = order.CustomerWalletDeduction.HasValue && order.CustomerWalletDeduction.Value > 0 ? order.CustomerWalletDeduction.Value : 0;
        var discounts = order.OrderSubTotalDiscountExclTax + order.OrderDiscount;
        var amountNotPaidInCash = paidWithWallet + paidOnAccount + discounts;
        var amountPaidInCash = order.OrderTotal;

        // order total is reduced during checkout, so add the amount not paid in cash
        var fullOrderTotal = amountPaidInCash + amountNotPaidInCash;

        // deduct existing refunds / credit transactions from order total
        var customerWallet = (await _customerWalletService.GetAllCustomerWalletsAsync(order.CustomerId, storeId: order.StoreId)).FirstOrDefault();
        if (customerWallet != null)
        {
            var creditTransactions = await _customerWalletService.GetAllCustomerWalletTransactionsAsync(customerWallet.Id, WalletTransactionType.Credit, order.TransactionTrackingNumber, true, storeId: order.StoreId);
            fullOrderTotal -= creditTransactions.Sum(i => i.Amount);
        }

        var refundResponse = new ArchRefundResponse();
        switch (request.refund_type)
        {
            default:
                return CreateResponse(ArchRefundStatusMessage.UnableToCapture);
            case 0:
                refundResponse = await FinalizeTransactionAsync(request, order, fullOrderTotal, paidOnAccount);
                break;
            case 1:
                refundResponse = await RefundTransactionAsync(request, order, fullOrderTotal, paidOnAccount);
                break;
        }

        if (refundResponse.status == ArchRefundStatusMessage.RefundSuccessfullyCaptured.Key)
        {
            var customerWalletTransactions = await _customerWalletService.GetAllCustomerWalletTransactionsAsync(customerWallet.Id, WalletTransactionType.Credit, order.TransactionTrackingNumber, storeId: order.StoreId);

            foreach (var transaction in customerWalletTransactions)
            {
                if (transaction.ArchRefundType == null)
                {
                    transaction.ArchRefundType = request.refund_type;
                    await _customerWalletService.UpdateCustomerWalletTransactionAsync(transaction);
                }
            }
            if (request.refund_type == 0)
            {
                order.OrderStatus = OrderStatus.Picked;
                await _orderService.UpdateOrderAsync(order);
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Order #{order.Id} has been picked",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
        }

        return refundResponse;
    }

    public async Task<ArchRefundResponse> FinalizeTransactionAsync(ArchRefundRequest request, Order order, decimal fullOrderTotal, decimal paidOnAccount)
    {
        var refundResponse = new ArchRefundResponse();
        var refundAmount = fullOrderTotal - request.total;
        refundAmount -= paidOnAccount;
        refundAmount = refundAmount < 0 ? 0 : refundAmount;

        var amountExceedsQuotation = request.total > fullOrderTotal;
        if (amountExceedsQuotation)
            return CreateResponse(ArchRefundStatusMessage.GivenAmountExceedsQuotation);

        var givenAmountEqualsQuotation = request.total == fullOrderTotal;
        if (givenAmountEqualsQuotation)
        {
            await _eventPublisher.PublishAsync(new ArchValidateEvent(order));
            refundResponse = CreateResponse(ArchRefundStatusMessage.GivenAmountEqualsQuotationAmount);
        }

        var isFullRefund = request.total == 0;
        var giveCashback = request.total > 0;
        if (isFullRefund || giveCashback)
        {
            var description = $"Arch finalize {request.transaction_code} / Order total { await _priceFormatter.FormatPriceAsync(fullOrderTotal, true, false)} / Arch total {await _priceFormatter.FormatPriceAsync(request.total, true, false)}";
            await _paymentService.ArchRefundAsync(order, description, request.refund_type, refundAmount);

            refundResponse.status = ArchRefundStatusMessage.RefundSuccessfullyCaptured.Key;
            refundResponse.message = ArchRefundStatusMessage.RefundSuccessfullyCaptured.Value;
        }

        var isSuccessfulRefund = refundResponse.status == ArchRefundStatusMessage.RefundSuccessfullyCaptured.Key || refundResponse.status == ArchRefundStatusMessage.GivenAmountEqualsQuotationAmount.Key;
        if (isSuccessfulRefund)
        {
            JobManager.RunAt(() => ProcessInvoiceFromArchAsync(order.Id, refundAmount, isFullRefund), new TimeSpan(0, 0, 2, 0));
        }

        return refundResponse;
    }

    public async Task ProcessInvoiceFromArchAsync(int orderId, decimal refundAmount, bool isFullRefund)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (!isFullRefund)
        {
            await _eventPublisher.PublishAsync(new ArchValidateEvent(order));
        }

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        await _workflowMessageService.SendCustomerCashbackNotificationMessageAsync(customer, order, _localizationSettings.DefaultAdminLanguageId, refundAmount);

        if (order.ShippingStatus == ShippingStatus.NotYetShipped)
        {
            // Arch validation process will not be asked for in terms of a customer who is a debtor with arch
            // i.e., not a cash debtor not an anonymous customer.
            // We need to ensure that the OTP is still sent out under this condition. 
            if (customer.IsDebtor(order.StoreId))
            {
                // send OTP to customer for the now validated order
                await _eventPublisher.PublishAsync(new ArchValidateEvent(order));
            }

            await _orderProcessingService.UpdateToReadyForCollectionAsync(order, shipment: null);
            _archApiBase = EngineContext.Current.Resolve<ArchApiBase>();
            if (_archApiBase != null)
            {
                _archApiBase.SetStoreID(order.StoreId);
                _archApiBase.EnsureInvoice(order.TransactionTrackingNumber, order.StoreId);
            }
        }

        if (isFullRefund)
        {
            order.OrderStatus = OrderStatus.Cancelled;
            await _orderService.UpdateOrderAsync(order);
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = $"Order #{order.Id} has been updated to cancelled in Arch",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
        }
    }

    public async Task<ArchRefundResponse> RefundTransactionAsync(ArchRefundRequest request, Order order, decimal fullOrderTotal, decimal paidOnAccount)
    {
        var refundAmount = fullOrderTotal;
        fullOrderTotal -= paidOnAccount;

        var amountExceedsQuotation = request.total > fullOrderTotal;
        if (amountExceedsQuotation)
            return CreateResponse(ArchRefundStatusMessage.GivenAmountExceedsQuotation);

        var giveCashback = fullOrderTotal > 0;
        if (giveCashback)
        {
            var description = $"Arch refund {request.transaction_code}" +
                                            $" / Order total {await _priceFormatter.FormatPriceAsync(fullOrderTotal, true, false)}" +
                                            $" / Arch total {await _priceFormatter.FormatPriceAsync(request.total, true, false)}";
            await _paymentService.ArchRefundAsync(order, description, request.refund_type, refundAmount);

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            await _workflowMessageService.SendCustomerCashbackNotificationMessageAsync(customer, order, _localizationSettings.DefaultAdminLanguageId, refundAmount);

            order.OrderStatusId = (int)OrderStatus.Cancelled;
            await _orderService.UpdateOrderAsync(order);

            return CreateResponse(ArchRefundStatusMessage.RefundSuccessfullyCaptured);
        }

        return CreateResponse(ArchRefundStatusMessage.UnableToCapture);
    }
}
