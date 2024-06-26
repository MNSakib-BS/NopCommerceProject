using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Core.Domain.Orders;
using OrderShippingMethodCapacityMapping = Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain.OrderShippingMethodCapacityMapping;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Events
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        public OrderPlacedEventConsumer(IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService, IGenericAttributeService genericAttributeService, IWorkContext workContext)
        {
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }



        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            //if (!this._deliverySchedulingSetting.EnablePlugin || !this._pluginValidityCheckerService.CheckIsValid(typeof(DeliverySchedulingSetting), "Resanehlab.OrderFulfillment.DeliveryScheduling").IsValid || eventMessage.Order == null || eventMessage.Order.ShippingStatus == 10)
            //    return;
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _orderShippingMethodCapacityMappingService.InsertAsync(new OrderShippingMethodCapacityMapping()
            {
                OrderId = ((BaseEntity)eventMessage.Order).Id,
                ShippingMethodCapacityId = await _genericAttributeService.GetAttributeAsync<int>(customer, "ShippingMethodCapacityId", 0, 0),
                DeliveryDateOnUtc = await _genericAttributeService.GetAttributeAsync<DateTime>(customer, "DeliveryDateOnUtc", 0, new DateTime())
            });
        }
    }
}
