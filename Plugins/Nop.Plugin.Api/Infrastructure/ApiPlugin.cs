using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Domain;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Api.Infrastructure
{
    public class ApiPlugin : BasePlugin
    {
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public ApiPlugin(
            ISettingService settingService,
            IWorkContext workContext,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IWebHelper webHelper)
        {
            _settingService = settingService;
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _webHelper = webHelper;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ApiAdmin/Settings";
        }

        public override async Task InstallAsync()
        {
            //locales

            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api", "Api plugin");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.Configure", "Configure Web Api");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.GeneralSettings", "General Settings");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.EnableApi", "Enable Api");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.EnableApi.Hint",
                                                                 "By checking this settings you can Enable/Disable the Web Api");

            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.Menu.Title", "API");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.Menu.Settings.Title", "Settings");

            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.Page.Settings.Title", "Api Settings");


            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.Settings.GeneralSettingsTitle", "General Settings");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Api.Admin.Edit", "Edit");

            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.Categories.Fields.Id.Invalid", "Id is invalid");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.InvalidPropertyType", "Invalid Property Type");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.InvalidType", "Invalid {0} type");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.InvalidRequest", "Invalid request");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.InvalidRootProperty", "Invalid root property");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.NoJsonProvided", "No Json provided");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.InvalidJsonFormat", "Json format is invalid");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.Category.InvalidImageAttachmentFormat", "Invalid image attachment base64 format");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.Category.InvalidImageSrc", "Invalid image source");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Api.Category.InvalidImageSrcType", "You have provided an invalid image source/attachment ");

            _settingService.SaveSetting(new ApiSettings());

            var apiRole = await _customerService.GetCustomerRoleBySystemNameAsync(Constants.Roles.ApiRoleSystemName);

            if (apiRole == null)
            {
                apiRole = new CustomerRole
                          {
                              Name = Constants.Roles.ApiRoleName,
                              Active = true,
                              SystemName = Constants.Roles.ApiRoleSystemName
                          };

               await _customerService.InsertCustomerRoleAsync(apiRole);
            }
            else if (apiRole.Active == false)
            {
                apiRole.Active = true;
               await _customerService.UpdateCustomerRoleAsync(apiRole);
            }


           await base.InstallAsync();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the Install function, but just to be safe we will made web.config changes after the plugin was installed.
            //_webConfigMangerHelper.AddConfiguration();
        }

        public override async Task UninstallAsync()
        {
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Api");

            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Title");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title");

            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Configure");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.GeneralSettings");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint");

            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle");
            //_localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Edit");


            var apiRole =await _customerService.GetCustomerRoleBySystemNameAsync(Constants.Roles.ApiRoleSystemName);
            if (apiRole != null)
            {
                apiRole.Active = false;
                await _customerService.UpdateCustomerRoleAsync(apiRole);
            }


           await base.UninstallAsync();
        }

        public async void ManageSiteMap(SiteMapNode rootNode)
        {
            var pluginMenuName =await _localizationService.GetResourceAsync("Plugins.Api.Admin.Menu.Title", _workContext.GetWorkingLanguageAsync().Id, defaultValue: "API");

            var settingsMenuName =await _localizationService.GetResourceAsync("Plugins.Api.Admin.Menu.Settings.Title", _workContext.GetWorkingLanguageAsync().Id, defaultValue: "API");

            const string adminUrlPart = "Admin/";

            var pluginMainMenu = new SiteMapNode
                                 {
                                     Title = pluginMenuName,
                                     Visible = true,
                                     SystemName = "Api-Main-Menu",
                                     IconClass = "fa-genderless"
                                 };

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
                                          {
                                              Title = settingsMenuName,
                                              Url = _webHelper.GetStoreLocation() + adminUrlPart + "ApiAdmin/Settings",
                                              Visible = true,
                                              SystemName = "Api-Settings-Menu",
                                              IconClass = "fa-genderless"
                                          });


            rootNode.ChildNodes.Add(pluginMainMenu);
        }
    }
}
