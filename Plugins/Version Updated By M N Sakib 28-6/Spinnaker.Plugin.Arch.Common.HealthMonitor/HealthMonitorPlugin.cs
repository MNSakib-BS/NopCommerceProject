using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor
{
    public class CustomPlugin : BasePlugin, IWidgetPlugin
    {
        protected readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public CustomPlugin(ISettingService settingService, IWebHelper webHelper)
        {
            _settingService = settingService;
            _webHelper = webHelper;
        }
        public bool HideInWidgetList => true;


        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductDetailsTop });
        }
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/HealthMonitor/Configure";
        }

     

        Type IWidgetPlugin.GetWidgetViewComponent(string widgetZone)
        {
            return typeof (ProductViewTracker);
        }
        public override async Task InstallAsync()
        {
            var settings = new HealthMonitorSettings
            {
                MonitoringHostURL = "http://localhost",
                SiteKey = "sitekey"
            };

            await _settingService.SaveSettingAsync(settings);

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var keyValuePairs = PluginResources();
            foreach (var keyValuePair in keyValuePairs)
            {
                await localizationService.AddOrUpdateLocaleResourceAsync(keyValuePair.Key, keyValuePair.Value);
            }
            /*await base.InstallAsync();*/
        }

        public List<KeyValuePair<string, string>> PluginResources()
        {
            var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("Admin.Spinnaker.HealthMonitor.MonitoringHostURL", "Host URL"));
            list.Add(new KeyValuePair<string, string>("Admin.Spinnaker.HealthMonitor.SiteKey", "SiteKey"));
            return list;
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

    }
}
