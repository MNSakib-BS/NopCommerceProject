using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Spinnaker.Plugin.Arch.Common.HealthMonitor.Areas.Admin.Models;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Areas.Admin.Controllers;
[HttpsRequirement]
public class HealthController : BaseAdminController
{
    private readonly ISettingService _settingService;
    private IEventPublisher _eventPublisher;
    private readonly INotificationService _notificationService;
    public HealthController(ISettingService settingService, 
                            IEventPublisher eventPublisher, 
                            INotificationService notificationService)
    {
        _settingService = settingService;
        _eventPublisher = eventPublisher;
        _notificationService = notificationService;
    }
    public async Task<IActionResult> Configure()
    {
        var _cdnSettings = await _settingService.LoadSettingAsync<HealthMonitorSettings>(0);
        var model = new ConfigurationModel { MonitoringHostURL = _cdnSettings.MonitoringHostURL, SiteKey = _cdnSettings.SiteKey };
        return View("~/Plugins/Arch.HealthMonitor/Areas/Admin/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        var _healthSettings = await _settingService.LoadSettingAsync<HealthMonitorSettings>();
        _healthSettings.SiteKey = model.SiteKey;
        _healthSettings.MonitoringHostURL = model.MonitoringHostURL;
        await _settingService.SaveSettingAsync(_healthSettings);
        return await Configure();
    }

    [HttpPost]
    public async Task<IActionResult> SendTest()
    {   
        //TODO: need Correction 
        //await _eventPublisher.PublishAsync(new ErrorHandledEvent($"Testing event publishing", new Exception("Test")));
        _notificationService.SuccessNotification("Test notification sent");

        return await Configure();
    }

}
