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
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.DeliveryScheduling;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Components
{
    [ViewComponent(Name = "OrderAdminSummaryDeliveryDate")]
    public class OrderAdminSummaryDeliveryDateViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private DeliverySchedulingSettings _settings;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
        private readonly ILocalizationService _localizationService;

        public OrderAdminSummaryDeliveryDateViewComponent(
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

        public async Task<IViewComponentResult> InvokeAsync(object additionalData)
        {
            // Added settings initialization here
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync(); // Await the async call
             _settings = _settingService.LoadSetting<DeliverySchedulingSettings>(storeScope); // Initialize settings here

            OrderModel orderModel = (OrderModel)additionalData;

            OrderShippingMethodCapacityMapping mappingByOrderId = await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(orderModel.Id);

            if (mappingByOrderId == null)
                return (IViewComponentResult)((ViewComponent)this).Content(string.Empty);

            ShippingMethodCapacity shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityAsync(mappingByOrderId.ShippingMethodCapacityId);

            if (shippingMethodCapacity == null)
                return (IViewComponentResult)((ViewComponent)this).Content(string.Empty);

            AvailableDeliveryDateTimeRange deliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);

            if (deliveryDateTimeRange == null)
                return (IViewComponentResult)((ViewComponent)this).Content(string.Empty);

            AvailableDeliveryTimeRange deliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeAsync(deliveryDateTimeRange.AvailableDeliveryTimeRangeId);

            if (deliveryTimeRange == null)
                return (IViewComponentResult)((ViewComponent)this).Content(string.Empty);

            DeliverySchedulingOrderModel schedulingOrderModel1 = new DeliverySchedulingOrderModel();

            schedulingOrderModel1.DeliveryTimeRange = mappingByOrderId != null ? mappingByOrderId.DeliveryDateOnUtc.ToString("dddd, MMMM dd, yyyy") : await this._localizationService.GetResourceAsync("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.NoData");
            DeliverySchedulingOrderModel schedulingOrderModel2 = schedulingOrderModel1;
            string str;

            if (mappingByOrderId == null)
                str = await _localizationService.GetResourceAsync("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.NoData");
            else
                str = deliveryTimeRange.Time.ToString();

            schedulingOrderModel2.DeliveryTime = str;
            schedulingOrderModel1.ShippingMethodName = mappingByOrderId != null ? orderModel.ShippingMethod : await this._localizationService.GetResourceAsync("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.NoData");
            var themeName = await EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync(); // Await the async call

            return View($"~/Plugins/Arch.DeliveryScheduling/Themes/{themeName}/Views/OrderDeliveryDateTime.cshtml", schedulingOrderModel1);
        }
    }
}
