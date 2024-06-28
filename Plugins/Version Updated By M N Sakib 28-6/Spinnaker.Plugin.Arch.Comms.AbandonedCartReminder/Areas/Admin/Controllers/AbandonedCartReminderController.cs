using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Spinnaker.Plugin.Arch.AbandonedCartReminder.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Stores;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Models.AbandonedCartReminder;

namespace Spinnaker.Plugin.Arch.AbandonedCartReminder.Areas.Admin.Controllers
{
    public class AbandonedCartReminderController : BaseAdminController
    {
        private readonly IPermissionService PermissionService;
        private readonly IAbandonedCartReminderService AbandonedCartReminderService;
        private readonly IShoppingCartService ShoppingCartService;
        private readonly IStoreMappingService StoreMappingService;
        private readonly IStoreService StoreService;

        public AbandonedCartReminderController(IAbandonedCartReminderService abandonedCartReminderService,
                                                IPermissionService permissionService,
                                                IShoppingCartService shoppingCartService,
                                                IStoreMappingService storeMappingService,
                                                IStoreService storeService)
        {
            AbandonedCartReminderService = abandonedCartReminderService;
            PermissionService = permissionService;
            ShoppingCartService = shoppingCartService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return await AccessDeniedDataTablesJson();

            AbandonedCartReminderSearchModel model = await AbandonedCartReminderService.PrepareSearchModelAsync(new AbandonedCartReminderSearchModel());
            return View(model);
        }

        public async Task<IActionResult> FormList(AbandonedCartReminderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return await AccessDeniedDataTablesJson();

            AbandonedCartReminderListModel model = await AbandonedCartReminderService.PrepareFormListModelAsync(searchModel);
            return  Json(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await AbandonedCartReminderService.PrepareReminderModelAsync(new AbandonedCartReminderModel());
            return  View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Create(AbandonedCartReminderModel model, bool continueEditing)
        {
            //update picture seo file name
            var saveModel = await AbandonedCartReminderService.AddFormAsync(model);
            saveModel.SelectedStoreIds = model.SelectedStoreIds;
            var abandonedCartReminder = saveModel.ToEntity<Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder>();

            await SaveStoreMappingsAsync(abandonedCartReminder, saveModel);

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = saveModel.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await AbandonedCartReminderService.GetAbandonedCartReminderFormAsync(id);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(AbandonedCartReminderModel model, bool continueEditing)
        {
            model = await AbandonedCartReminderService.EditFormAsync(model);
            if (!continueEditing)
                return RedirectToAction("List");
            var reminder = await AbandonedCartReminderService.GetAbandonedCartReminderByIdAsync(model.Id);

            await SaveStoreMappingsAsync(reminder, model);
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder abandonedCartReminder)
        {
            if (abandonedCartReminder != null)
            {
                await AbandonedCartReminderService.DeleteAbandonedCartReminderAsync(abandonedCartReminder);
            }
            return RedirectToAction("List");
        }

        #region "Store Mapping"
        protected virtual async Task SaveStoreMappingsAsync(Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder reminder, AbandonedCartReminderModel model)
        {
            reminder.LimitedToStores = model.SelectedStoreIds.Any();
            await AbandonedCartReminderService.UpdateReminderAsync(reminder);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(reminder);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await StoreMappingService.InsertStoreMappingAsync(reminder, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }
        #endregion
    }
}
