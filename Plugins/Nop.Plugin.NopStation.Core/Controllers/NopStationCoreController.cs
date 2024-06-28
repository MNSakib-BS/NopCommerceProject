using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.Core.Models;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Core.Controllers
{
    public class NopStationCoreController : BaseAdminController
    {
        private readonly IStoreContext _storeContext;
        private readonly INopStationLicenseService _licenseService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly INopStationPluginManager _nopStationPluginManager;

        public NopStationCoreController(IStoreContext storeContext,
            INopStationLicenseService licenseService, 
            INotificationService notificationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext,
            IBaseAdminModelFactory baseAdminModelFactory,
            INopStationPluginManager nopStationPluginManager)
        {
            _storeContext = storeContext;
            _licenseService = licenseService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
            _baseAdminModelFactory = baseAdminModelFactory;
            _nopStationPluginManager = nopStationPluginManager;
        }

        [NopStationLicense]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            return RedirectToAction("License");
        }

        [NopStationLicense]
        public async Task<IActionResult> LocaleResource()
        {
            if (!await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            var searchModel = new CoreLocaleResourceSearchModel();
            searchModel.SearchLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id;
            await _baseAdminModelFactory.PrepareLanguagesAsync(searchModel.AvailableLanguages, false);

            var plugins = await _nopStationPluginManager.LoadNopStationPluginsAsync(storeId: _storeContext.GetCurrentStoreAsync().Id);
            foreach (var item in plugins)
            {
                searchModel.AvailablePlugins.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                {
                    Value = item.PluginDescriptor.SystemName,
                    Text = item.PluginDescriptor.FriendlyName
                });
            }
            searchModel.AvailablePlugins.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Value = "",
                Text = _localizationService.GetResourceAsync("Admin.NopStation.Core.Resources.List.SearchPluginSystemName.All").Result
            });
            
            return View(searchModel);
        }

        [HttpPost, NopStationLicense]
        public async Task<IActionResult> LocaleResource(CoreLocaleResourceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageConfiguration))
                return await AccessDeniedDataTablesJson();

            var resources = await _nopStationPluginManager.LoadPluginStringResourcesAsync(searchModel.SearchPluginSystemName,
            searchModel.SearchResourceName, searchModel.SearchLanguageId, _storeContext.GetCurrentStoreAsync().Id,
                searchModel.Page - 1, searchModel.PageSize);

            var model = new CoreLocaleResourceListModel().PrepareToGrid(searchModel, resources, () =>
            {
                return resources.Select(resource =>
                {
                    return new CoreLocaleResourceModel()
                    {
                        ResourceName = resource.Key.ToLower(),
                        ResourceValue = resource.Value,
                        ResourceNameLanguageId = $"{resource.Key}___{searchModel.SearchLanguageId}"
                    };
                });
            });

            return Json(model);
        }

        [HttpPost, NopStationLicense]
        public async Task<JsonResult> ResourceUpdate(CoreLocaleResourceModel model)
        {
            if (!_permissionService.AuthorizeAsync(CorePermissionProvider.ManageConfiguration).Result)
                return await AccessDeniedDataTablesJson();

            if (string.IsNullOrWhiteSpace(model.ResourceNameLanguageId))
                return ErrorJson(_localizationService.GetResourceAsync("Admin.NopStation.Core.Resources.FailedToSave"));

            var token = model.ResourceNameLanguageId.Split(new[] { "___" }, StringSplitOptions.None);
            model.ResourceName = token[0];
            model.LanguageId = int.Parse(token[1]);

            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            var resource = _localizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId).Result;

            if (resource != null)
            {
                resource.ResourceValue = model.ResourceValue;
                await _localizationService.UpdateLocaleStringResourceAsync(resource);
            }
            else
            {
                var rs = model.ToEntity<LocaleStringResource>();
                rs.LanguageId = model.LanguageId;
                await _localizationService.InsertLocaleStringResourceAsync(rs);
            }

            return new NullJsonResult();
        }

        public async Task<IActionResult> PluginInfo()
        {
            if (!await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageLicense))
                return AccessDeniedView();

            var plugins = await _nopStationPluginManager.LoadNopStationPluginsAsync(storeId: _storeContext.GetCurrentStoreAsync().Id);
            var model = plugins.Select(x => new PluginInfoModel()
            {
                FileName = x.PluginDescriptor.AssemblyFileName,
                Version = x.PluginDescriptor.Version,
                Name = x.PluginDescriptor.FriendlyName
            });

            return View(model);
        }

        public async Task<IActionResult> License()
        {
            if (!await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageLicense))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();

            var model = new LicenseModel();
            model.ActiveStoreScopeConfiguration = storeId;

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult>License(LicenseModel model)
        {
            if (!await _permissionService.AuthorizeAsync(CorePermissionProvider.ManageLicense))
                return AccessDeniedView();

            var result = _licenseService.VerifyProductKey(model.LicenseString);

            switch (result)
            {
                case KeyVerificationResult.InvalidProductKey:
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.NopStation.Core.License.InvalidProductKey"));
                    return View(model);
                case KeyVerificationResult.InvalidForDomain:
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.NopStation.Core.License.InvalidForDomain"));
                    return View(model);
                case KeyVerificationResult.InvalidForNOPVersion:
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.NopStation.Core.License.InvalidForNOPVersion"));
                    return View(model);
                case KeyVerificationResult.Valid:
                    var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                    var settings = _settingService.LoadSetting<NopStationCoreSettings>(storeId);

                    settings.LicenseStrings.Add(model.LicenseString);
                    _settingService.SaveSetting(settings);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.NopStation.Core.License.Saved"));

                    return RedirectToAction("License");
                default:
                    return RedirectToAction("License");
            }
        }
    }
}
