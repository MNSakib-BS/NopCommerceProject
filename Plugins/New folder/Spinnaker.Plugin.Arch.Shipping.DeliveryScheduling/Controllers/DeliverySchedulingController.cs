using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Web.Framework.Controllers;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.DeliveryScheduling;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Controllers
{
    public class DeliverySchedulingController : BasePluginController
    {
        private readonly DeliverySchedulingSettings _deliverySchedulingSettings;
        private readonly IWorkContext _workContext;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;

        public DeliverySchedulingController(
            DeliverySchedulingSettings deliverySchedulingSettings,
            IWorkContext workContext,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IGenericAttributeService genericAttributeService,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService)
        {
            _deliverySchedulingSettings = deliverySchedulingSettings;
            _workContext = workContext;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _genericAttributeService = genericAttributeService;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
        }

        [HttpPost]
        public async Task<IActionResult> CheckAvailabilityForSelectedDateTimes(
            int shippingMethodCapacityId,
            string deliveryDateOnUtc,
            int availableDeliveryTimeRangeId,
            string deliveryTime,
            string selectedShippingOption)
        {
            DateTime parsedDate;
            bool isValidDate = DateTime.TryParseExact(deliveryDateOnUtc, "dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

            if (!isValidDate)
            {
                return Json(new { Success = false, Error = "Invalid date format" });
            }

            parsedDate = parsedDate.Date;

           
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _genericAttributeService.SaveAttributeAsync<DateTime>(customer, "DeliveryDateOnUtc", parsedDate, 0);

   
            ShippingMethodCapacity shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityAsync(shippingMethodCapacityId);

            if (shippingMethodCapacity == null)
                throw new NullReferenceException("shippingMethodCapacity");

            
            int placedOrdersCount = await _orderShippingMethodCapacityMappingService.GetPlacedOrdersCountAsync(parsedDate, shippingMethodCapacityId);

           
            if (shippingMethodCapacity.Capacity - placedOrdersCount <= 0)
                return Json(new { Success = false });
            
            await _genericAttributeService.SaveAttributeAsync<int>(customer, "ShippingMethodCapacityId", shippingMethodCapacityId, 0);
            await _genericAttributeService.SaveAttributeAsync<DateTime>(customer, "DeliveryDateOnUtc", parsedDate, 0);
            await _genericAttributeService.SaveAttributeAsync<string>(customer, "DeliveryTime", deliveryTime, 0);

            return Json(new { Success = true });
        }
    }
}
