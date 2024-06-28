using System.Linq;
using ExCSS;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Home;

namespace Nop.Plugin.Arch.Core.Areas.Admin.Controllers
{
    public partial class ArchHomeController : BaseAdminController
    {
        #region Fields

        protected readonly AdminAreaSettings _adminAreaSettings;
        protected readonly ICommonModelFactory _commonModelFactory;
        protected readonly IHomeModelFactory _homeModelFactory;
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly ISettingService _settingService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ICustomerService _customerService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ArchHomeController(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            IHomeModelFactory homeModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            _adminAreaSettings = adminAreaSettings;
            _commonModelFactory = commonModelFactory;
            _homeModelFactory = homeModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _customerService = customerService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            //display a warning to a store owner if there are some error
            var customer = await _workContext.GetCurrentCustomerAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if ((hideCard || closeCard) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            {
                var warnings = await _commonModelFactory.PrepareSystemWarningModelsAsync();
                if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail || warning.Level == SystemWarningLevel.Warning))
                {
                    var locale = await _localizationService.GetResourceAsync("Admin.System.Warnings.Errors");
                    _notificationService.WarningNotification(string.Format(locale, Url.Action("Warnings", "Common")), false); //do not encode URLs
                }
            }
            //progress of localization 
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var progress = await _genericAttributeService.GetAttributeAsync<string>(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute);
            if (!string.IsNullOrEmpty(progress))
            {
                var locale = await _localizationService.GetResourceAsync("Admin.Configuration.LanguagePackProgressMessage");
                _notificationService.SuccessNotification(string.Format(locale, progress, NopLinksDefaults.OfficialSite.Translations), false);
                await _genericAttributeService.SaveAttributeAsync(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute, string.Empty);
            }

            //prepare model
            var model = await _homeModelFactory.PrepareDashboardModelAsync(new DashboardModel());

            return View(model);
            
        }

     [HttpPost]
    public virtual async Task<IActionResult> NopCommerceNewsHideAdv()
    {
        _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
        await _settingService.SaveSettingAsync(_adminAreaSettings);

        return Content("Setting changed");
    }


        #endregion
    }
}