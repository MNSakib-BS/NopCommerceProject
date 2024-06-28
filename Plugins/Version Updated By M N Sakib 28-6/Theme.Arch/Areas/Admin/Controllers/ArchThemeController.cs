using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.Theme.Arch.Areas.Admin.Models;
using Nop.Plugin.NopStation.Theme.Arch.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Themes;

namespace Nop.Plugin.NopStation.Theme.Arch.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class ArchThemeController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IThemeContext _themeContext;
        private readonly INopFileProvider _nopFileProvider;

        #endregion

        #region Ctor

        public ArchThemeController(IPermissionService permissionService,
            IWorkContext workContext,
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ILogger logger,
            IThemeContext themeContext,
            INopFileProvider nopFileProvider)
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _logger = logger;
            _themeContext = themeContext;
            _nopFileProvider = nopFileProvider;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var themeSettings = await _settingService.LoadSettingAsync<ArchThemeSettings>(storeId);
            var model = themeSettings.ToSettingsModel<ConfigurationModel>();

            var defaultColor = themeSettings.CustomThemeColor;
            if (string.IsNullOrEmpty(defaultColor))
                model.CustomThemeColor = ArchThemeDefaults.DefaultThemeColor;

            var accentColor = themeSettings.AccentThemeColor;
            if (string.IsNullOrEmpty(accentColor))
                model.AccentThemeColor = ArchThemeDefaults.AccentThemeColor;

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return View(model);

            model.CustomThemeColor_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.CustomThemeColor, storeId);
            model.AccentThemeColor_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.AccentThemeColor, storeId);
            model.CustomCss_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.CustomCss, storeId);
            model.EnableImageLazyLoad_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.EnableImageLazyLoad, storeId);
            model.FooterEmail_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.FooterEmail, storeId);
            model.FooterLogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.FooterLogoPictureId, storeId);
            model.LazyLoadPictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.LazyLoadPictureId, storeId);
            model.ShowLogoAtPageFooter_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.ShowLogoAtPageFooter, storeId);
            model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.ShowSupportedCardsPictureAtPageFooter, storeId);
            model.SupportedCardsPictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.SupportedCardsPictureId, storeId);
            model.ShowLoginPictureAtLoginPage_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.ShowLoginPictureAtLoginPage, storeId);
            model.LoginPictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.LoginPictureId, storeId);

            #region Description Box Settings
            model.EnableDescriptionBoxOne_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.EnableDescriptionBoxOne, storeId);
            model.DescriptionBoxOneTitle_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxOneTitle, storeId);
            model.DescriptionBoxOneText_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxOneText, storeId);
            model.DescriptionBoxOneUrl_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxOneUrl, storeId);
            model.DescriptionBoxOnePictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxOnePictureId, storeId);

            model.EnableDescriptionBoxTwo_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.EnableDescriptionBoxTwo, storeId);
            model.DescriptionBoxTwoTitle_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxTwoTitle, storeId);
            model.DescriptionBoxTwoText_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxTwoText, storeId);
            model.DescriptionBoxTwoUrl_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxTwoUrl, storeId);
            model.DescriptionBoxTwoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxTwoPictureId, storeId);

            model.EnableDescriptionBoxThree_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.EnableDescriptionBoxThree, storeId);
            model.DescriptionBoxThreeTitle_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxThreeTitle, storeId);
            model.DescriptionBoxThreeText_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxThreeText, storeId);
            model.DescriptionBoxThreeUrl_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxThreeUrl, storeId);
            model.DescriptionBoxThreePictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxThreePictureId, storeId);

            model.EnableDescriptionBoxFour_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.EnableDescriptionBoxFour, storeId);
            model.DescriptionBoxFourTitle_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxFourTitle, storeId);
            model.DescriptionBoxFourText_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxFourText, storeId);
            model.DescriptionBoxFourUrl_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxFourUrl, storeId);
            model.DescriptionBoxFourPictureId_OverrideForStore = await _settingService.SettingExistsAsync(themeSettings, x => x.DescriptionBoxFourPictureId, storeId);
            #endregion

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var ArchSettings = await _settingService.LoadSettingAsync<ArchThemeSettings>(storeScope);

            //if (ArchSettings.CustomThemeColor != model.CustomThemeColor)
            //{
            if (string.IsNullOrEmpty(model.CustomThemeColor))
                await ReplaceThemeColor(ArchThemeDefaults.DefaultThemeColor);
            else
            {
                var hexPattern = new Regex("^#([A-Fa-f0-9]{6})$");
                var color = hexPattern.Match(model.CustomThemeColor);
                if (color.Length > 0)
                {
                    var colorHexCode = color.Value;
                    await ReplaceThemeColor(colorHexCode);
                }
            }
            //}

            // if (ArchSettings.AccentThemeColor != model.AccentThemeColor)
            // {
            if (string.IsNullOrEmpty(model.AccentThemeColor))
                await ReplaceAccentThemeColorAsync(ArchThemeDefaults.AccentThemeColor);
            else
            {
                var hexPattern = new Regex("^#([A-Fa-f0-9]{6})$");
                var color = hexPattern.Match(model.AccentThemeColor);
                if (color.Length > 0)
                {
                    var colorHexCode = color.Value;
                    await ReplaceAccentThemeColorAsync(colorHexCode);
                }
            }
            //}

            ArchSettings = model.ToSettings(ArchSettings);

            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.CustomThemeColor, model.CustomThemeColor_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.AccentThemeColor, model.AccentThemeColor_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.CustomCss, model.CustomCss_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.EnableImageLazyLoad, model.EnableImageLazyLoad_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.FooterEmail, model.FooterEmail_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.FooterLogoPictureId, model.FooterLogoPictureId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.LazyLoadPictureId, model.LazyLoadPictureId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.ShowLogoAtPageFooter, model.ShowLogoAtPageFooter_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.ShowSupportedCardsPictureAtPageFooter, model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.SupportedCardsPictureId, model.SupportedCardsPictureId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.ShowLoginPictureAtLoginPage, model.ShowLoginPictureAtLoginPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.LoginPictureId, model.LoginPictureId_OverrideForStore, storeScope, false);

            #region Footer Description Box Settings
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.EnableDescriptionBoxOne, model.EnableDescriptionBoxOne_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxOneTitle, model.DescriptionBoxOneTitle_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxOneText, model.DescriptionBoxOneText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxOneUrl, model.DescriptionBoxOneUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxOnePictureId, model.DescriptionBoxOnePictureId_OverrideForStore, storeScope, false);

            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.EnableDescriptionBoxTwo, model.EnableDescriptionBoxTwo_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxTwoTitle, model.DescriptionBoxTwoTitle_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxTwoText, model.DescriptionBoxTwoText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxTwoUrl, model.DescriptionBoxTwoUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxTwoPictureId, model.DescriptionBoxTwoPictureId_OverrideForStore, storeScope, false);

            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.EnableDescriptionBoxThree, model.EnableDescriptionBoxThree_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxThreeTitle, model.DescriptionBoxThreeTitle_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxThreeText, model.DescriptionBoxThreeText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxThreeUrl, model.DescriptionBoxThreeUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxThreePictureId, model.DescriptionBoxThreePictureId_OverrideForStore, storeScope, false);

            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.EnableDescriptionBoxFour, model.EnableDescriptionBoxFour_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxFourTitle, model.DescriptionBoxFourTitle_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxFourText, model.DescriptionBoxFourText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxFourUrl, model.DescriptionBoxFourUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(ArchSettings, x => x.DescriptionBoxFourPictureId, model.DescriptionBoxFourPictureId_OverrideForStore, storeScope, false);
            #endregion

            await _settingService.ClearCacheAsync();

            //success
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
        private async Task UpdateLocales(ConfigurationModel model)
        {
            var storeScope = _storeContext.GetCurrentStoreAsync().Id;
            if (model.Locales.Count == 0)
            {
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTitleValue", model.DescriptionBoxOneTitle, storeScope, model.DescriptionBoxOneTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTextValue", model.DescriptionBoxOneText, storeScope, model.DescriptionBoxOneText_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTitleValue", model.DescriptionBoxTwoTitle, storeScope, model.DescriptionBoxTwoTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTextValue", model.DescriptionBoxTwoText, storeScope, model.DescriptionBoxTwoText_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTitleValue", model.DescriptionBoxThreeTitle, storeScope, model.DescriptionBoxThreeTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTextValue", model.DescriptionBoxThreeText, storeScope, model.DescriptionBoxThreeText_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTitleValue", model.DescriptionBoxFourTitle, storeScope, model.DescriptionBoxFourTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTextValue", model.DescriptionBoxFourText, storeScope, model.DescriptionBoxFourText_OverrideForStore);
            }
            foreach (var locale in model.Locales)
            {
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTitleValue", locale.DescriptionBoxOneTitle, model.DescriptionBoxOneTitle, storeScope, model.DescriptionBoxOneTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTextValue", locale.DescriptionBoxOneText, model.DescriptionBoxOneText, storeScope, model.DescriptionBoxOneText_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTitleValue", locale.DescriptionBoxTwoTitle, model.DescriptionBoxTwoTitle, storeScope, model.DescriptionBoxTwoTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTextValue", locale.DescriptionBoxTwoText, model.DescriptionBoxTwoText, storeScope, model.DescriptionBoxTwoText_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTitleValue", locale.DescriptionBoxThreeTitle, model.DescriptionBoxThreeTitle, storeScope, model.DescriptionBoxThreeTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTextValue", locale.DescriptionBoxThreeText, model.DescriptionBoxThreeText, storeScope, model.DescriptionBoxThreeText_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTitleValue", locale.DescriptionBoxFourTitle, model.DescriptionBoxFourTitle, storeScope, model.DescriptionBoxFourTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTextValue", locale.DescriptionBoxFourText, model.DescriptionBoxFourText, storeScope, model.DescriptionBoxFourText_OverrideForStore);
            }
        }
        private async Task SetLocaleStringResource(int languageId, string resourceName, string resourceValue, string settingValue, int storeScope = 0, bool saveStoreSpecificValue = false)
        {
            if (storeScope > 0)
            {
                if (saveStoreSpecificValue)
                {
                    resourceName = $"{resourceName}-{storeScope}";
                }
                else
                {
                    var localeString = _localizationService.GetLocaleStringResourceByName($"{resourceName}-{storeScope}", languageId, false);
                    if (localeString != null && localeString.Id > 0)
                    {
                        _localizationService.DeleteLocaleStringResource(localeString);
                    }
                }
            }
            var localizedResource = _localizationService.GetLocaleStringResourceByName(resourceName, languageId, false);
            if (string.IsNullOrEmpty(settingValue))
            {
                settingValue = string.Empty;
            }
            if (string.IsNullOrEmpty(resourceValue))
            {
                resourceValue = settingValue;
            }
            if (localizedResource == null)
            {
                localizedResource = new LocaleStringResource
                {
                    LanguageId = languageId,
                    ResourceName = resourceName,
                    ResourceValue = resourceValue
                };
               await _localizationService.InsertLocaleStringResourceAsync(localizedResource);
            }
            else
            {
                localizedResource.ResourceValue = resourceValue;
                _localizationService.UpdateLocaleStringResource(localizedResource);
            }
        }
        private async Task SetDefaultLocaleStringResource(string resourceName, string settingValue, int storeScope = 0, bool saveStoreSpecificValue = false)
        {
            if (storeScope > 0 && saveStoreSpecificValue)
            {
                resourceName = $"{resourceName}-{storeScope}";
            }
            var localizedResource =await _localizationService.GetLocaleStringResourceByNameAsync(resourceName );
            if (string.IsNullOrEmpty(settingValue))
            {
                settingValue = string.Empty;
            }
            if (localizedResource == null)
            {
                localizedResource = new LocaleStringResource
                {
                    LanguageId = _workContext.GetWorkingLanguageAsync().Id,
                    ResourceName = resourceName,
                    ResourceValue = settingValue
                };
                _localizationService.InsertLocaleStringResourceAsync(localizedResource);
            }
            else
            {
                localizedResource.ResourceValue = settingValue;
                _localizationService.UpdateLocaleStringResource(localizedResource);
            }
        }

        #endregion

        #region Utilities

        protected async Task ReplaceThemeColor(string colorHexCode)
        {
            try
            {
                var workingThemeName =await _themeContext.GetWorkingThemeNameAsync();
                var colorCssFile = $"~/Themes/{workingThemeName}/Content/css/color.css";
                var fileSystemPath = _nopFileProvider.MapPath(colorCssFile);
                if (_nopFileProvider.FileExists(fileSystemPath))
                {
                    var text = System.IO.File.ReadAllText(fileSystemPath);
                    text = Regex.Replace(text, "--okred: #([A-Fa-f0-9]{6});", $"--okred: {colorHexCode};");
                    System.IO.File.WriteAllText(fileSystemPath, text);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Could not save theme color", e);
            }
        }
        protected async Task ReplaceAccentThemeColorAsync(string colorHexCode)
        {
            try
            {
                var workingThemeName =await _themeContext.GetWorkingThemeNameAsync();
                var colorCssFile = $"~/Themes/{workingThemeName}/Content/css/color.css";
                var fileSystemPath = _nopFileProvider.MapPath(colorCssFile);
                if (_nopFileProvider.FileExists(fileSystemPath))
                {
                    var text = System.IO.File.ReadAllText(fileSystemPath);
                    text = Regex.Replace(text, "--okgreen: #([A-Fa-f0-9]{6});", $"--okgreen: {colorHexCode};");
                    System.IO.File.WriteAllText(fileSystemPath, text);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Could not save accent color", e);
            }
        }

        #endregion
    }
}
