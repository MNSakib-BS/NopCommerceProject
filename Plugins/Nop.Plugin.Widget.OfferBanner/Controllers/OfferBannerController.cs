using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widget.OfferBanner.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widget.OfferBanner.Controllers;
public class OfferBannerController : BasePluginController
{

    private IPermissionService _permissionService;
    private IStoreContext _storeContext;
    private ISettingService _settingService;
    
    public OfferBannerController(
        IPermissionService permissionService,
        IStoreContext storeContext,
        ISettingService settingService)
    {

        _permissionService = permissionService;
        _storeContext = storeContext;
        _settingService = settingService;
    }

    [AuthorizeAdmin]
    [Area("Admin")]
    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var offerBannerSettings = await _settingService.LoadSettingAsync<OfferBannerSettings>(storeScope);
        var model = new ConfigurationModel()
        {
            Message = offerBannerSettings.Message
        };

        return View(model);
    }

    [AuthorizeAdmin]
    [Area("Admin")]
    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel offerBannerSettingsModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var offerBannerSettings = await _settingService.LoadSettingAsync<OfferBannerSettings>(storeScope);

        if (ModelState.IsValid)
        {
            offerBannerSettings.Message = offerBannerSettingsModel.Message;
            await _settingService.SaveSettingAsync(offerBannerSettings);
            return await Configure();
        }

        return BadRequest();
    }
}
