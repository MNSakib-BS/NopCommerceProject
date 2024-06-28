using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Spinnaker.Plugin.Arch.Shipping.AddressPicker.Components;
using System.Collections.Generic;

namespace Spinnaker.Plugin.Arch.AddressPicker
{
 
    public class CustomPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => true;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(AddressPickerViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.Footer });

        }
        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }
}
