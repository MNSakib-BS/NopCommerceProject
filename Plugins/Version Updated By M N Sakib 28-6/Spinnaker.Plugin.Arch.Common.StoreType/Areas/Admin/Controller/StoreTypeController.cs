using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Models.StoreType;
using Spinnaker.Plugin.Arch.Common.StoreType.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc;

namespace Spinnaker.Plugin.Arch.Common.StoreType.Areas.Admin.Controller;

public class StoreTypeController : BaseAdminController
{
    private readonly IStoreTypeService StoreTypeService;
    private readonly IPermissionService PermissionService;
    private readonly IStoreService StoreService;

    public StoreTypeController(IStoreTypeService storeTypeService, IPermissionService permissionService, IStoreService storeService)
    {
        StoreTypeService = storeTypeService;
        PermissionService = permissionService;
        StoreService = storeService;
    }

    public async Task<IActionResult> Index()
    {
        return RedirectToAction("List");
    }

    public async Task<IActionResult> List()
    {
        if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
            return (IActionResult)AccessDeniedDataTablesJson();

        StoreTypeSearchModel model = await StoreTypeService.PrepareSearchModelAsync(new StoreTypeSearchModel());
        return View(model);
    }

    public async Task<IActionResult> FormList(StoreTypeSearchModel searchModel)
    {
        if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
            return (IActionResult)AccessDeniedDataTablesJson();

        StoreTypeListModel model = await StoreTypeService.PrepareFormListModelAsync(searchModel);
        return Json(model);
    }

    public IActionResult Create()
    {
        var model = new StoreTypeModel();
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Create(StoreTypeModel model, bool continueEditing)
    {
        var storeType = model.ToEntity<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>();
        model = await StoreTypeService.AddFormAsync(model);

        if (!continueEditing)
            return RedirectToAction("List");

        return RedirectToAction("Edit", new { id = model.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await StoreTypeService.GetStoreTypeFormAsync(id);
        await BuildStoreList();
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Edit(StoreTypeModel model, bool continueEditing)
    {
        model = await StoreTypeService.EditFormAsync(model);
        if (!continueEditing)
            return RedirectToAction("List");

        return RedirectToAction("Edit", new { id = model.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType storeType)
    {
        if (storeType != null)
        {
            await StoreTypeService.DeleteStoreTypeAsync(storeType);
        }
        return RedirectToAction("List");
    }

    #region "StoreMapping"
    public async Task BuildStoreList()
    {
        ViewBag.Stores = await StoreTypeService.GetStoresAsync();
    }

    [HttpPost]
    public async Task<IActionResult> StoreTypeMappingList(int storeTypeId, StoreTypeMappingSearchModel searchModel)
    {
        if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
            return (IActionResult)AccessDeniedDataTablesJson();

        var storeType = await StoreTypeService.GetStoreTypeByIdAsync(storeTypeId) ?? throw new ArgumentException("No store Type found with the specified id");

        var model = await StoreTypeService.PrepareStoreTypeMappingListModelAsync(searchModel, storeType);

        return Json(model);
    }

    public async Task<IActionResult> StoreTypeMappingAdd(string storeId, int storeTypeId)
    {
        if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
            return AccessDeniedView();

        var storeIdValue = 0;

        if (!string.IsNullOrEmpty(storeId))
        {
            storeIdValue = int.Parse(storeId);
        }

        if (storeIdValue != 0 && storeTypeId != 0)
        {
            var storeType = await StoreTypeService.GetStoreTypeByIdAsync(storeTypeId) ?? throw new ArgumentException("No store type found with the specified id");

            if (await StoreTypeService.GetMappingByStoreTypeIdAsync(storeTypeId, storeIdValue) != null)
                return Json(new { Result = false });

            var store = await StoreService.GetStoreByIdAsync(storeIdValue) ?? throw new ArgumentException("No store found with the specified id");

            var storeTypeMapping = await StoreTypeService.GetMappingByStoreIdAsync(store.Id);
            if (storeTypeMapping == null)
            {
                await StoreTypeService.InsertStoreTypeMappingAsync(new StoreTypeMapping
                {
                    StoreId = storeIdValue,
                    StoreTypeId = storeTypeId
                });
            }
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    public async Task<IActionResult> StoreTypeMappingDelete(int id)
    {
        if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
            return AccessDeniedView();

        var storeTypeMapping = await StoreTypeService.GetStoreTypeMappingByIdAsync(id) ?? throw new ArgumentException("No customer picture found with the specified id");
        await StoreTypeService.DeleteStoreTypeMappingAsync(storeTypeMapping);

        return new NullJsonResult();
    }
    #endregion
}
