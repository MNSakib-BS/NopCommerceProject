using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using Spinnaker.Plugin.Arch.Common.StoreType.Areas.Admin.Infrastructure;

namespace Spinnaker.Plugin.Arch.Common.StoreType
{
    public class CustomPlugin : BasePlugin, IPlugin, IAdminMenuPlugin

    {
        private readonly List<string> _widgetZones;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IPluginDefaults _pluginDefaults;
        
        public CustomPlugin(IWebHelper webHelper, ILocalizationService localizationService, IPluginDefaults pluginDefaults)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
            _pluginDefaults = pluginDefaults;
        }

        public bool HideInWidgetList => true;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(ProductViewTracker);
        }
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductDetailsTop });
        }
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var siteMapNode1 = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");
            if (siteMapNode1 != null)
            {
                var menuItem = new SiteMapNode()
                {
                    Title = "Store Type",
                    Visible = true,
                    IconClass = "fa fa-dot-circle-o",
                    Url = "/Admin/StoreType",
                    ControllerName = "StoreType",
                    ActionName = "Index",
                    SystemName = "StoreType",
                    RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
                };

                siteMapNode1.ChildNodes.Add(menuItem);
            }


     
        }
        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
       
        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {

           await base.UpdateAsync(currentVersion, targetVersion);
           await _pluginDefaults.UpdatePluginResourcesAsync(currentVersion, targetVersion);
        }

    }
}
