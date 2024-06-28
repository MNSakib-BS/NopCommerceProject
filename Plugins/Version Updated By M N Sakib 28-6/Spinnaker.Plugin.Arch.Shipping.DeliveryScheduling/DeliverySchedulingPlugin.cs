using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling
{
    public class DeliverySchedulingPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
