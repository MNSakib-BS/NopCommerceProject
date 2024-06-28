using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Catalog;
using System.Linq;
using System.Threading.Tasks;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Factories;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;

namespace Acumen.Plugin.Widget.DeliveryScheduling.Components
{
    [ViewComponent(Name = "WidgetsDeliveryScheduling")]
    public class WidgetsDeliverySchedulingViewComponent : NopViewComponent
    {

        private readonly IDeliveryScheduleFactory _deliveryScheduleFactory;



        public WidgetsDeliverySchedulingViewComponent(
            IDeliveryScheduleFactory deliveryScheduleFactory)
        {
            _deliveryScheduleFactory = deliveryScheduleFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            return await System.Threading.Tasks.Task.Run<IViewComponentResult>(() =>
            {
                var model = _deliveryScheduleFactory.PrepareModelAsync(new AvailableDeliveryDateTimeModel(), null);

                var themeName = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync;

                return View($"~/Plugins/Arch.DeliveryScheduling/Themes/{themeName}/Views/DeliverySchedule.cshtml", model);
            });
        }
    }
}
