using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Themes;
using System.Threading.Tasks;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Factories;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using System.Linq.Expressions;
using System;
using Nop.Web.Models.Order;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.DeliveryScheduling;

namespace Acumen.Plugin.Widget.DeliveryScheduling.Components
{
    [ViewComponent(Name = "LoadOrderHistoryDetailsDeliveryDateTimeViewComponent")]
    public class LoadOrderHistoryDetailsDeliveryDateTimeViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private DeliverySchedulingSettings _settings;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
        private readonly ILocalizationService _localizationService;

        public LoadOrderHistoryDetailsDeliveryDateTimeViewComponent(
            ISettingService settingService,
            IStoreContext storeContext,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            ILocalizationService localizationService,
            IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService)
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _localizationService = localizationService;
            _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
        }

        
        private async Task InitializeAsync()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            _settings = _settingService.LoadSetting<DeliverySchedulingSettings>(storeScope);
        }

        public async Task<IViewComponentResult> InvokeAsync(object additionalData)
        {
           
            await InitializeAsync();

            OrderDetailsModel orderDetails = (OrderDetailsModel)additionalData;

            OrderShippingMethodCapacityMapping mappingByOrderId = await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(orderDetails.Id);

            if (mappingByOrderId == null)
                return Content(string.Empty);

            ShippingMethodCapacity shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityAsync(mappingByOrderId.ShippingMethodCapacityId);
            if (shippingMethodCapacity == null)
                return Content(string.Empty);

            AvailableDeliveryDateTimeRange deliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);
            if (deliveryDateTimeRange == null)
                return Content(string.Empty);

            AvailableDeliveryTimeRange deliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeAsync(deliveryDateTimeRange.AvailableDeliveryTimeRangeId);
            if (deliveryTimeRange == null)
                return Content(string.Empty);

            OrderDetailsDeliveryDateTimeModel deliveryDateTimeModel = new OrderDetailsDeliveryDateTimeModel
            {
                DeliveryDate = mappingByOrderId.DeliveryDateOnUtc.ToString("dddd, MMMM dd, yyyy"),
                DeliveryTime = deliveryTimeRange.Time.ToString()
            };

            var themeName = await EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync();

            return View($"~/Plugins/Arch.DeliveryScheduling/Themes/{themeName}/Views/LoadOrderHistoryDetailsDeliveryDateTime.cshtml", deliveryDateTimeModel);
        }
    }
}
