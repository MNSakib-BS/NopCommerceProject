using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Framework.Themes;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.DeliveryScheduling;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Factories;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using NUglify.JavaScript.Syntax;

public class OrderSummaryDeliveryDateViewComponent : ViewComponent
{
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly ICatalogModelFactory _catalogModelFactory;
    private readonly IDeliveryScheduleFactory _deliveryScheduleFactory;
    private readonly DeliverySchedulingSettings _deliverySchedulingSetting;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly OrderSettings _orderSettings;

    public OrderSummaryDeliveryDateViewComponent(
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext,
        ICatalogModelFactory catalogModelFactory,
        IDeliveryScheduleFactory deliveryScheduleFactory,
        DeliverySchedulingSettings deliverySchedulingSetting,
        IGenericAttributeService genericAttributeService,
        OrderSettings orderSettings)
    {
        _settingService = settingService;
        _storeContext = storeContext;
        _workContext = workContext;
        _catalogModelFactory = catalogModelFactory;
        _deliveryScheduleFactory = deliveryScheduleFactory;
        _deliverySchedulingSetting = deliverySchedulingSetting;
        _genericAttributeService = genericAttributeService;
        _orderSettings = orderSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var model = new OrderDetailsDeliveryDateTimeModel
        {
           
            DeliveryDate =(await _genericAttributeService.GetAttributeAsync<DateTime>(customer, "DeliveryDateOnUtc", 0, new DateTime()))
                                                   .ToString("dddd, MMMM dd, yyyy"),
            DeliveryTime =( await _genericAttributeService.GetAttributeAsync<DateTime>(customer, "DeliveryTime", 0, new DateTime())).ToString("dddd, MMMM dd, yyyy"),
            IsOpc = _orderSettings.OnePageCheckoutEnabled
        };

        var themeName = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync();

        return View($"~/Plugins/Arch.DeliveryScheduling/Themes/{themeName}/Views/SelectedDeliverySchedule.cshtml", model);
    }
}
