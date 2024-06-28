using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Spinnaker.Plugin.Arch.Shipping.AddressValidator.Components;

namespace Spinnaker.Plugin.Arch.Shipping.AddressValidator
{
    public class AddressValidatorPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;


        public AddressValidatorPlugin(ILocalizationService localizationService,
            IWebHelper webHelper,
            ISettingService settingService)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
            _settingService = settingService;
        }
        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(AddressValidatorViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            
            return Task.FromResult<IList<string>>(new List<string> { "AddressValidatorWidgetZone" });

        }
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Addressvalidator/Configure";
        }
        public override async Task InstallAsync()
        {
            var settings = new AddressValidatorSettings
            {
                CountryRestriction = "ZA"
            };

           await _settingService.SaveSettingAsync(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Arch.Addressvalidator.Instructions"] = "Instructions",
                ["Plugins.Arch.Addressvalidator.SelectedCountryCodes"] = "Selected Countries",
                ["Plugins.Arch.Addressvalidator.SelectedCountryCodes.Hint"] = "Address validation results returned from the API will be filtered using the selected country's two letter iso codes."
            });
            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
           await _settingService.DeleteSettingAsync<AddressValidatorSettings>();

            //locales
           await _localizationService.DeleteLocaleResourcesAsync("Plugins.Arch.Addressvalidator");

            await base.UninstallAsync();
        }
    }
}
