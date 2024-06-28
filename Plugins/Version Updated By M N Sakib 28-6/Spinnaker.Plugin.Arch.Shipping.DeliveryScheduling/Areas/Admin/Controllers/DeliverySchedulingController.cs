using Microsoft.AspNetCore.Mvc;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Factories;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services;
using Nop.Web.Areas.Admin.Models.Customers;
using Microsoft.AspNetCore.Http;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Services.Helpers;
//using Nop.Web.API;
//using FluentAssertions;
using static LinqToDB.Reflection.Methods.LinqToDB;
using Nop.Core.Domain.Shipping;
//using Microsoft.WindowsAzure.Storage.File;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Common;
using Nop.Services.Stores;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.DeliveryScheduling;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    public class DeliverySchedulingController : BasePluginController
    {

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IDeliveryScheduleConfigurationFactory _deliveryScheduleConfigurationFactory;
        private readonly ITimeDeliveryService _timeDeliveryService;
        private readonly IShippingMethodService _shippingMethodService;
        private readonly IStoreService _storeService;


        public DeliverySchedulingController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService, ISettingService settingService,
            IStoreContext storeContext,
            ITimeDeliveryService timeDeliveryService,
            IDeliveryScheduleConfigurationFactory deliveryScheduleConfigurationFactory, IShippingMethodService shippingMethodService, IStoreService storeService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _timeDeliveryService = timeDeliveryService;
            _deliveryScheduleConfigurationFactory = deliveryScheduleConfigurationFactory;
            _shippingMethodService = shippingMethodService;
            _storeService = storeService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var storeScope =await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var stores = await _storeService.GetAllStoresAsync();
            var settings = await _settingService.LoadSettingAsync<DeliverySchedulingSettings>(storeScope);

            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.value, y => y.id);

            var currentWidgetZones = (from i in (settings.WidgetZones ?? "").Split(';')
                                      where lookup.ContainsKey(i)
                                      select lookup[i]).ToList();

            var availableWidgetZones = from wzd in widgetZonesData
                                       select new SelectListItem
                                       {
                                           Text = wzd.value,
                                           Value = wzd.id.ToString(),
                                           Selected = currentWidgetZones.Contains(wzd.id)
                                       };

            var model = new ConfigurationModel
            {
                WidgetZones = currentWidgetZones,
                AvailableWidgetZones = availableWidgetZones.ToList(),
                HourOffset = settings.HourOffset,
                DayOffset = settings.DayOffset
            };

            model.TimeRangeSearchModel = await _deliveryScheduleConfigurationFactory.PrepareTimeRangeSearchModelAsync(model.TimeRangeSearchModel);
            model.ShippingMethods = await _deliveryScheduleConfigurationFactory.PrepareAvailableShippingMethodsAsync();

            model.StoreCount = stores.Count;
            model.StoreId = storeScope;

            return View("~/Plugins/Arch.DeliveryScheduling/Areas/Admin/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [IgnoreAntiforgeryToken]
        [Area(AreaNames.ADMIN)]
        [HttpPost, ActionName("ShowDeliveryCapacitySchedule")]
        public async Task<IActionResult> ShowDeliveryCapacitySchedule(int shippingMethodId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return await AccessDeniedDataTablesJson();

            var model = new ShippingMethodAvailableDateCapacity();

            model = await _deliveryScheduleConfigurationFactory.PrepareShippingMethodCapacityModelAsync(model, shippingMethodId);

            return View("~/Plugins/Arch.DeliveryScheduling/Areas/Admin/Views/_DateTimeCapacity.cshtml", model);
        }

        [AuthorizeAdmin]
        [IgnoreAntiforgeryToken]
        [Area(AreaNames.ADMIN)]
        [HttpPost, ActionName("UpdateCapacity")]
        public async Task<IActionResult> UpdateCapacity([FromBody] Dictionary<int, int> capacities)
        {
            foreach (var item in capacities)
            {
                var shippingCapacity = await _shippingMethodService.GetShippingCapacityByIdAsync(item.Key);
                shippingCapacity.Capacity = item.Value;
                await _shippingMethodService.UpdateShippingMethodCapacityAsync(shippingCapacity);
            }

            return Json(new { success = true, message = await _localizationService.GetResourceAsync("Capacity Updated Successfully") });

        }

        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        [HttpPost, ActionName("TimeRangeList")]
        public virtual async Task<IActionResult> TimeRangeList(TimeRangeSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return await AccessDeniedDataTablesJson();

            var listModel = await _deliveryScheduleConfigurationFactory.PrepareTimeRangeListModelAsync(model);

            return Json(listModel);
        }


        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return BadRequest();

            if (!ModelState.IsValid)
                return await Configure();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<DeliverySchedulingSettings>(storeScope);

            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.value, y => y.id);

            var availableWidgetZones = from wzd in widgetZonesData
                                       select new SelectListItem
                                       {
                                           Text = wzd.value,
                                           Value = wzd.id.ToString(),
                                           Selected = model.WidgetZones.Contains(wzd.id)
                                       };

            var selectedWidgetZones = availableWidgetZones.Where(i => i.Selected).Select(i => i.Text).ToList();

            var widgetZones = new StringBuilder();
            foreach (var widgetZone in selectedWidgetZones)
            {
                widgetZones.Append(widgetZone);
                if (selectedWidgetZones.IndexOf(widgetZone) < selectedWidgetZones.Count - 1)
                {
                    widgetZones.Append(";");
                }
            }
            if (model.DayOffset > 0)
                settings.DayOffset = model.DayOffset;

            if (model.HourOffset > 0)
                settings.HourOffset = model.HourOffset;

            settings.WidgetZones = widgetZones.ToString();

            await _settingService.SaveSettingAsync(settings, storeScope);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }



        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        [HttpPost, ActionName("create")]
        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //prepare model
            var model = await _deliveryScheduleConfigurationFactory.PrepareModelAsync(new AvailableDeliveryTimeRangeModel(), null);

            return View("~/Plugins/Arch.DeliveryScheduling/Areas/Admin/Views/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(AvailableDeliveryTimeRangeModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var timeRange = model.ToEntity<AvailableDeliveryTimeRange>();

            Enum.TryParse(model.StartTime, out TimeRangeEnum startTimeEnum);
            Enum.TryParse(model.EndTime, out TimeRangeEnum endTimeEnum);


            var startDescription = startTimeEnum.GetDescription();
            var endDescription = endTimeEnum.GetDescription();

            var timeFormatting = $"{startDescription}{await _localizationService.GetResourceAsync("Admin.AvailableDeliveryTimeRangeModel.TimeRangeDelim")}{endDescription}";

            timeRange.Time = timeFormatting;
            timeRange.StoreId = await _storeContext.GetActiveStoreScopeConfigurationAsync();

            await _timeDeliveryService.InsertTimeRangeAsync(timeRange);

            var deliveryDays = Enum.GetValues(typeof(DeliveryDayEnum));

            var shippingMethods = await _shippingMethodService.GetAllAsync();

            foreach (var day in deliveryDays)
            {
                var availableDeliveryDateTimeRange = new AvailableDeliveryDateTimeRange
                {
                    AvailableDeliveryTimeRangeId = timeRange.Id,
                    DayOfWeek = (int)day,
                    StartDateOnUtc = DateTime.UtcNow,
                    EndDateOnUtc = DateTime.UtcNow
                };

                await _timeDeliveryService.InsertDateTimeRangeAsync(availableDeliveryDateTimeRange);

                foreach (var shippingMethod in shippingMethods)
                {
                    var shippingMethodCapacity = new ShippingMethodCapacity
                    {
                        AvailableDeliveryDateTimeRangeId = availableDeliveryDateTimeRange.Id,
                        Deleted = timeRange.Deleted,
                        ShippingMethodId = shippingMethod.Id,
                    };

                    await _shippingMethodService.InsertShippingMethodCapacityAsync(shippingMethodCapacity);
                }
            }

            if (continueEditing)
                return RedirectToAction("Edit", new { id = timeRange.Id });

            return RedirectToAction("Configure", new { area = "Admin" });
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var timeRange = await _timeDeliveryService.GetByIdAsync(id);

            if (timeRange == null || timeRange.Deleted)
                return RedirectToAction("Configure", new { area = "Admin" });

            var model = await _deliveryScheduleConfigurationFactory.PrepareModelAsync(new AvailableDeliveryTimeRangeModel(), timeRange);

            return View("~/Plugins/Arch.DeliveryScheduling/Areas/Admin/Views/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(AvailableDeliveryTimeRangeModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var timeRange = await _timeDeliveryService.GetByIdAsync(model.Id);
            if (timeRange == null || timeRange.Deleted)
                return RedirectToAction("Configure", new { area = "Admin" });

            //TODO: add fluent validation

            Enum.TryParse(model.StartTime, out TimeRangeEnum startTimeEnum);
            Enum.TryParse(model.EndTime, out TimeRangeEnum endTimeEnum);

            var startDescription = startTimeEnum.GetDescription();
            var endDescription = endTimeEnum.GetDescription();

            var timeFormatting = $"{startDescription}{await _localizationService.GetResourceAsync("Admin.AvailableDeliveryTimeRangeModel.TimeRangeDelim")}{endDescription}";

            timeRange.Time = timeFormatting;
            timeRange.DisplayOrder = model.DisplayOrder;

            await _timeDeliveryService.UpdateTimeRangeAsync(timeRange);

            if (continueEditing)
                return RedirectToAction("Edit", new { id = timeRange.Id });

            return RedirectToAction("Configure", new { area = "Admin" });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var timeRange = await _timeDeliveryService.GetByIdAsync(id);
            var timeDeliveryDateRanges = await _timeDeliveryService.GetAllDeliveryDateTimeRangeAsync(timeRange.Id);

            if (timeRange == null)
                return RedirectToAction("Configure", new { area = "Admin" });

            try
            {
                await _timeDeliveryService.DeleteTimeRangeAsync(timeRange);

                foreach (var item in timeDeliveryDateRanges)
                {
                    await _shippingMethodService.DeleteShippingMethodCapacityByDeliveryDateIdAsync(item.Id);
                }

                _notificationService.SuccessNotification("Time Range has been deleted successfully.");
                return RedirectToAction("Configure", new { area = "Admin" });
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = id });
            }
        }

        private List<(string name, string value, int id)> GetWidgetZoneData()
        {
            int id = 1000;
            return typeof(Nop.Web.Framework.Infrastructure.PublicWidgetZones)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .OrderBy(x => x.Name)
                .Select(x => (name: x.Name, value: x.GetValue(null, null).ToString(), id++))
                .ToList();
        }
    }
}
