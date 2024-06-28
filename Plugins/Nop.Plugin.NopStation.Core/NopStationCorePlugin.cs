using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using Nop.Plugin.NopStation.Core.Services;
using System.Collections.Generic;
using Nop.Plugin.NopStation.Core.Helpers;
using System.Threading.Tasks;

namespace Nop.Plugin.NopStation.Core
{
    public class NopStationCorePlugin : BasePlugin, IAdminMenuPlugin, INopStationPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        public NopStationCorePlugin(IWebHelper webHelper,
            INopStationCoreService nopStationCoreService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _nopStationCoreService = nopStationCoreService;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/NopStationCore/Configure";
        }

        public override async Task InstallAsync()
        {
            await this.NopStationPluginInstallAsync(new CorePermissionProvider());
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await this.NopStationPluginUninstallAsync(new CorePermissionProvider());
            await base.UninstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageConfiguration))
            {
                //var settings = new SiteMapNode()
                //{
                //    Title = _localizationService.GetResource("Admin.NopStation.Core.Menu.Configuration"),
                //    Visible = true,
                //    IconClass = "fa-genderless",
                //    Url = "/Admin/NopStationCore/Configure",
                //    SystemName = "NopStationCore.Configuration"
                //};
                //_nopStationCoreService.ManageSiteMap(rootNode, settings, NopStationMenuType.Core);

                var resource = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.LocaleResources"),
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/Admin/NopStationCore/LocaleResource",
                    SystemName = "NopStationCore.LocaleResources"
                };
                await _nopStationCoreService.ManageSiteMapAsync(rootNode, resource, NopStationMenuType.Core);

                var system = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.PluginInfo"),
                    Visible = true,
                    IconClass = "fa-cog",
                    Url = "/Admin/NopStationCore/PluginInfo",
                    SystemName = "NopStationCore.PluginInfo"
                };
                await _nopStationCoreService.ManageSiteMapAsync(rootNode, system, NopStationMenuType.Root);
            }

            if (await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageLicense))
            {
                var license = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.License"),
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/Admin/NopStationCore/License",
                    SystemName = "NopStationCore.License"
                };
                await _nopStationCoreService.ManageSiteMapAsync(rootNode, license, NopStationMenuType.Core);
            }

            var reportBug = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.ReportBug"),
                Visible = true,
                IconClass = "fa-bug",
                Url = "https://www.nop-station.com/report-bug?utm_source=admin-panel",
                OpenUrlInNewTab = true
            };
            await _nopStationCoreService.ManageSiteMapAsync(rootNode, reportBug, NopStationMenuType.Root);
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.PluginInfo", "Nop-Station plugin information"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.LocaleResources", "String resources"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.License", "License"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.NopStation", "Nop Station"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.PluginInfo", "Plugin information"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.LocaleResources", "String resources"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.License", "License"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.Core", "Core settings"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.Themes", "Themes"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.Plugins", "Plugins"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Menu.ReportBug", "Report a bug"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.License.InvalidProductKey", "Your product key is not valid."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.License.InvalidForDomain", "Your product key is not valid for this domain."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.License.InvalidForNOPVersion", "Your product key is not valid for this nopCommerce version."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.License.Saved", "Your product key has been saved successfully."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.License.LicenseString", "License string"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.License.LicenseString.Hint", "Nop-station plugin/theme license string."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Common.Menu.Documentation", "Documentation"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.FailedToSave", "Failed to save resource string."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.Fields.Name", "Name"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.Fields.Value", "Value"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.List.SearchPluginSystemName", "Plugin"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.List.SearchPluginSystemName.Hint", "Search resource string by plugin."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.List.SearchResourceName", "Resource name"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.List.SearchResourceName.Hint", "Search resource string by resource name."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.List.SearchLanguageId", "Language"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.List.SearchLanguageId.Hint", "Search resource string by language."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Core.Resources.List.SearchPluginSystemName.All", "All"));

            return list;
        }
    }
}