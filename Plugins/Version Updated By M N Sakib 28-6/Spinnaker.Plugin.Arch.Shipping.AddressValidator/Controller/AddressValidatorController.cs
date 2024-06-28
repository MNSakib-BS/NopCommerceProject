using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using NUglify.Helpers;
using Spinnaker.Plugin.Arch.Shipping.AddressValidator;
using Spinnaker.Plugin.Arch.Shipping.AddressValidator.Models;


namespace Spinnaker.Plugin.Arch.Addressvalidator.Controllers
{
    [Area(AreaNames.ADMIN)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class AddressValidatorController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ICountryModelFactory _countryModelFactory;

        public AddressValidatorController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            ICountryModelFactory countryModelFactory)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _countryModelFactory = countryModelFactory;
        }


        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var addressValidatorSettings = await _settingService.LoadSettingAsync<AddressValidatorSettings>(storeScope);

            var countryListModel = await _countryModelFactory.PrepareCountryListModelAsync(new CountrySearchModel() { Start = 0, Length = 250 });
            var countryData = countryListModel.Data.ToList();
            var countrySelectList = new List<SelectListItem>();

            if (countryData != null && countryData.Count() > 0)
            {
                countrySelectList = countryData
                    .Select(x => new SelectListItem
                    {
                        Value = x.TwoLetterIsoCode,
                        Text = $"{x.Name}. {x.TwoLetterIsoCode}"
                    }).ToList();
            }

            var selectedCodes = new List<string>();
            if (!string.IsNullOrEmpty(addressValidatorSettings.CountryRestriction))
            {
                selectedCodes = addressValidatorSettings.CountryRestriction.ToUpper().Split(',').ToList();
                var selected = countrySelectList.Where(x => selectedCodes.Any(y => y.ToUpper() == x.Value));
                selected.ForEach(x => x.Selected = true);
            }

            var model = new ConfigurationModel
            {
                CountryRestriction = addressValidatorSettings.CountryRestriction.ToUpper(),
                AvailableCountries = countrySelectList,
                ActiveStoreScopeConfiguration = storeScope
            };

            return View("~/Plugins/Arch.Addressvalidator/Views/Configure.cshtml", model);
        }


        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope =await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var addressValidatorSettings = await _settingService.LoadSettingAsync<AddressValidatorSettings>(storeScope);
            var selectedCodes = string.Join(',', model.SelectedCountryCodes);
            addressValidatorSettings.CountryRestriction = selectedCodes;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            await _settingService.SaveSettingOverridablePerStoreAsync(addressValidatorSettings, x => x.CountryRestriction, model.CountryRestriction_OverrideForStore, storeScope, false);

            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
