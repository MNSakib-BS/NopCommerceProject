using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Arch.Core.Domains.Promotions;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arch.Core.Areas.Admin.Controllers
{
    public partial class ArchPromotionGroupController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPromotionGroupModelFactory _promotionGroupModelFactory;
        private readonly IPromotionGroupService _promotionGroupService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public ArchPromotionGroupController(IAclService aclService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPromotionGroupModelFactory promotionGroupModelFactory,
            IPromotionGroupService promotionGroupService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            IStoreContext storeContext)
        {
            _aclService = aclService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _promotionGroupModelFactory = promotionGroupModelFactory;
            _promotionGroupService = promotionGroupService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(PromotionGroup promotionGroup, PromotionGroupModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(promotionGroup,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(promotionGroup,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(promotionGroup,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(promotionGroup,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(promotionGroup,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(promotionGroup, localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(promotionGroup, seName, localized.LanguageId);
            }
        }

        protected virtual void UpdatePictureSeoNames(PromotionGroup promotionGroup)
        {
            var picture = _pictureService.GetPictureById(promotionGroup.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(promotionGroup.Name));

            picture = _pictureService.GetPictureById(promotionGroup.BannerPictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(promotionGroup.Name));
        }

        protected virtual void SavePromotionGroupAcl(PromotionGroup promotionGroup, PromotionGroupModel model)
        {
            promotionGroup.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            _promotionGroupService.UpdatePromotionGroup(promotionGroup);

            var existingAclRecords = _aclService.GetAclRecords(promotionGroup);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(promotionGroup, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected virtual void SaveStoreMappings(PromotionGroup promotionGroup, PromotionGroupModel model)
        {
            promotionGroup.LimitedToStores = model.SelectedStoreIds.Any();
            _promotionGroupService.UpdatePromotionGroup(promotionGroup);

            var existingStoreMappings = _storeMappingService.GetStoreMappings(promotionGroup);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(promotionGroup, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //prepare model
            var model = _promotionGroupModelFactory.PreparePromotionGroupSearchModel(new PromotionGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(PromotionGroupSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _promotionGroupModelFactory.PreparePromotionGroupListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //prepare model
            var model = _promotionGroupModelFactory.PreparePromotionGroupModel(new PromotionGroupModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(PromotionGroupModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var promotionGroup = model.ToEntity<PromotionGroup>();
                promotionGroup.CreatedOnUtc = DateTime.UtcNow;
                promotionGroup.UpdatedOnUtc = DateTime.UtcNow;
                _promotionGroupService.InsertPromotionGroup(promotionGroup);

                //search engine name
                model.SeName =await _urlRecordService.ValidateSeNameAsync(promotionGroup, model.SeName, promotionGroup.Name, true);
                await _urlRecordService.SaveSlugAsync(promotionGroup, model.SeName, 0);

                //locales
                UpdateLocales(promotionGroup, model);               

                //discounts
                var allDiscounts =await _discountService.GetAllDiscountsAsync(await _storeContext.GetActiveStoreScopeConfigurationAsync(), DiscountType.AssignedToPromotionGroups, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        //PromotionGroup.AppliedDiscounts.Add(discount);
                        _promotionGroupService.InsertDiscountPromotionGroupMapping(new DiscountPromotionGroupMapping { EntityId = promotionGroup.Id, DiscountId = discount.Id });

                }

                _promotionGroupService.UpdatePromotionGroup(promotionGroup);

                //update picture seo file name
                UpdatePictureSeoNames(promotionGroup);

                //ACL (customer roles)
                SavePromotionGroupAcl(promotionGroup, model);

                //stores
                SaveStoreMappings(promotionGroup, model);

                //promotion banner widget zones
                _promotionGroupModelFactory.PrepareWidgetZones(model, true);
                var selectedWidgetZones = model.AvailableWidgetZones.Where(i => i.Selected).Select(i => i.Text).ToList();
                _promotionGroupService.SaveWidgetZoneMapping(promotionGroup.Id, selectedWidgetZones);

                //activity log
               await _customerActivityService.InsertActivityAsync("AddNewPromotionGroup",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewPromotionGroup"), promotionGroup.Name), promotionGroup);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.PromotionGroups.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = promotionGroup.Id });
            }

            //prepare model
            model = _promotionGroupModelFactory.PreparePromotionGroupModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (! await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //try to get a PromotionGroup with the specified id
            var promotionGroup =await _promotionGroupService.GetPromotionGroupById(id);
            if (promotionGroup == null || promotionGroup.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _promotionGroupModelFactory.PreparePromotionGroupModel(null, promotionGroup);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(PromotionGroupModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //try to get a PromotionGroup with the specified id
            var promotionGroup = _promotionGroupService.GetPromotionGroupById(model.Id);
            if (promotionGroup == null || promotionGroup.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = promotionGroup.PictureId;
                promotionGroup = model.ToEntity(promotionGroup);
                promotionGroup.UpdatedOnUtc = DateTime.UtcNow;
                _promotionGroupService.UpdatePromotionGroup(promotionGroup);

                //search engine name
                model.SeName =await _urlRecordService.ValidateSeNameAsync(promotionGroup, model.SeName, promotionGroup.Name, true);
               await _urlRecordService.SaveSlugAsync(promotionGroup, model.SeName, 0);

                //locales
                UpdateLocales(promotionGroup, model);

                //discounts
                var allDiscounts =await _discountService.GetAllDiscountsAsync(await _storeContext.GetActiveStoreScopeConfigurationAsync(), DiscountType.AssignedToPromotionGroups, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (_promotionGroupService.GetDiscountAppliedToPromotionGroup(promotionGroup.Id, discount.Id) is null)
                            _promotionGroupService.InsertDiscountPromotionGroupMapping(new DiscountPromotionGroupMapping { EntityId = promotionGroup.Id, DiscountId = discount.Id });
                    }
                    else
                    {
                        //remove discount
                        if (_promotionGroupService.GetDiscountAppliedToPromotionGroup(promotionGroup.Id, discount.Id) is DiscountPromotionGroupMapping discountPromotionGroupMapping)
                            _promotionGroupService.DeleteDiscountPromotionGroupMapping(discountPromotionGroupMapping);
                    }
                }

                _promotionGroupService.UpdatePromotionGroup(promotionGroup);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != promotionGroup.PictureId)
                {
                    var prevPicture =await _pictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                       await _pictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(promotionGroup);

                //ACL
                SavePromotionGroupAcl(promotionGroup, model);

                //stores
                SaveStoreMappings(promotionGroup, model);

                //promotion banner widget zones
                _promotionGroupModelFactory.PrepareWidgetZones(model, true);
                var selectedWidgetZones = model.AvailableWidgetZones.Where(i => i.Selected).Select(i => i.Text).ToList();
                _promotionGroupService.SaveWidgetZoneMapping(promotionGroup.Id, selectedWidgetZones);

                //published
                CheckCanPublish(model);

                //activity log
               await _customerActivityService.InsertActivityAsync("EditPromotionGroup",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditPromotionGroup"), promotionGroup.Name), promotionGroup);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.PromotionGroups.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = promotionGroup.Id });
            }

            //prepare model
            model = _promotionGroupModelFactory.PreparePromotionGroupModel(model, promotionGroup, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        private void CheckCanPublish(PromotionGroupModel model)
        {
            var listModel = _promotionGroupModelFactory.PreparePromotionGroupProductListModel(model);
            model.Published = model.PictureId > 0 && listModel.Data.Count() > 0;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //try to get a PromotionGroup with the specified id
            var PromotionGroup = _promotionGroupService.GetPromotionGroupById(id);
            if (PromotionGroup == null)
                return RedirectToAction("List");

            _promotionGroupService.DeletePromotionGroup(PromotionGroup);

            //activity log
           await _customerActivityService.InsertActivityAsync("DeletePromotionGroup",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeletePromotionGroup"), PromotionGroup.Name), PromotionGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.PromotionGroups.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var PromotionGroups = _promotionGroupService.GetPromotionGroupsByIds(selectedIds.ToArray());
                _promotionGroupService.DeletePromotionGroups(PromotionGroups);

                PromotionGroups.ForEach(PromotionGroup => 
                {
                    //activity log
                    _customerActivityService.InsertActivity("DeletePromotionGroup",
                        string.Format(_localizationService.GetResource("ActivityLog.DeletePromotionGroup"), PromotionGroup.Name), PromotionGroup);
                });
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Products

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(PromotionGroupProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return await AccessDeniedDataTablesJson();

            //try to get a PromotionGroup with the specified id
            var PromotionGroup = _promotionGroupService.GetPromotionGroupById(searchModel.PromotionGroupId)
                ?? throw new ArgumentException("No PromotionGroup found with the specified id");

            //prepare model
            var model = _promotionGroupModelFactory.PreparePromotionGroupProductListModel(searchModel, PromotionGroup);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductUpdate(PromotionGroupProductModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //try to get a product PromotionGroup with the specified id
            var productPromotionGroup = _promotionGroupService.GetProductPromotionGroupById(model.Id)
                ?? throw new ArgumentException("No product PromotionGroup mapping found with the specified id");

            //fill entity from model
            productPromotionGroup = model.ToEntity(productPromotionGroup);
            _promotionGroupService.UpdateProductPromotionGroup(productPromotionGroup);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //try to get a product PromotionGroup with the specified id
            var productPromotionGroup = _promotionGroupService.GetProductPromotionGroupById(id)
                ?? throw new ArgumentException("No product PromotionGroup mapping found with the specified id");

            _promotionGroupService.DeleteProductPromotionGroup(productPromotionGroup);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAddPopup(int PromotionGroupId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //prepare model
            var model = _promotionGroupModelFactory.PrepareAddProductToPromotionGroupSearchModel(new AddProductToPromotionGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToPromotionGroupSearchModel searchModel)
        {
            if (! await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = _promotionGroupModelFactory.PrepareAddProductToPromotionGroupListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToPromotionGroupModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePromotionGroups))
                return AccessDeniedView();

            //get selected products
            var selectedProducts =await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingProductPromotionGroups = _promotionGroupService
                    .GetProductPromotionGroupsByPromotionGroupId(model.PromotionGroupId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //whether product PromotionGroup with such parameters already exists
                    if (_promotionGroupService.FindProductPromotionGroup(existingProductPromotionGroups, product.Id, model.PromotionGroupId) != null)
                        continue;

                    //insert the new product PromotionGroup mapping
                    _promotionGroupService.InsertProductPromotionGroup(new ProductPromotionGroup
                    {
                        PromotionGroupId = model.PromotionGroupId,
                        ProductId = product.Id,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToPromotionGroupSearchModel());
        }

        #endregion
    }
}