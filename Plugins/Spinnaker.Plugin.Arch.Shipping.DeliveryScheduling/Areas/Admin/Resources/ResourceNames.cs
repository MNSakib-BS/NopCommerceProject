using Nop.Services.Localization;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Resources
{
    public class ConfigurationResources
    {
        private static string _widgetZones = "Spinnaker.Plugin.Arch.Shipping.DeliverySchedulingg.Configuration.WidgetZones";

        [NopResourceDisplayName("Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Configuration.WidgetZones")]
        public string WidgetZones
        {
            get { return _widgetZones; }
            set { _widgetZones = value; }
        }
    }
}
