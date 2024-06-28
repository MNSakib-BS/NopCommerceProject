using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
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
    public partial class ArchRecommendedListController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IRecommendedListModelFactory _recommendedListModelFactory;
        private readonly IRecommendedListService _recommendedListService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public ArchRecommendedListController(ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IRecommendedListModelFactory recommendedListModelFactory,
            IRecommendedListService recommendedListService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IWorkContext workContext,
            IStoreContext storeContext)
        {
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _recommendedListModelFactory = recommendedListModelFactory;
            _recommendedListService = recommendedListService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productService = productService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        protected virtual void SaveStoreMappings(RecommendedList recommendedList, RecommendedListModel model)
        {
            recommendedList.LimitedToStores = model.SelectedStoreIds.Any();
            _recommendedListService.UpdateRecommendedList(recommendedList);

            var existingStoreMappings = _storeMappingService.GetStoreMappings(recommendedList);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(recommendedList, store.Id);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //prepare model
            var model = _recommendedListModelFactory.PrepareRecommendedListSearchModel(new RecommendedListSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(RecommendedListSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _recommendedListModelFactory.PrepareRecommendedListListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //prepare model
            var model = _recommendedListModelFactory.PrepareRecommendedListModel(new RecommendedListModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(RecommendedListModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var recommendedList = model.ToEntity<RecommendedList>();
                recommendedList.CreatedOnUtc = DateTime.UtcNow;
                recommendedList.UpdatedOnUtc = DateTime.UtcNow;
                _recommendedListService.InsertRecommendedList(recommendedList);

                //stores
                SaveStoreMappings(recommendedList, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewRecommendedList",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewRecommendedList"), recommendedList.Name), recommendedList);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.RecommendedLists.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = recommendedList.Id });
            }

            //prepare model
            model = _recommendedListModelFactory.PrepareRecommendedListModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //try to get a RecommendedList with the specified id
            var recommendedList = _recommendedListService.GetRecommendedListById(id);
            if (recommendedList == null)
                return RedirectToAction("List");

            //prepare model
            var model = _recommendedListModelFactory.PrepareRecommendedListModel(null, recommendedList);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(RecommendedListModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //try to get a RecommendedList with the specified id
            var recommendedList = _recommendedListService.GetRecommendedListById(model.Id);
            if (recommendedList == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                recommendedList = model.ToEntity(recommendedList);
                recommendedList.UpdatedOnUtc = DateTime.UtcNow;
                _recommendedListService.UpdateRecommendedList(recommendedList);

                //stores
                SaveStoreMappings(recommendedList, model);

                var listModel = _recommendedListModelFactory.PrepareRecommendedListProductListModel(model);
                model.Published = listModel.Data.Count() > 0;
                _recommendedListService.UpdateRecommendedList(recommendedList);

                //activity log
                _customerActivityService.InsertActivity("EditRecommendedList",
                    string.Format(_localizationService.GetResource("ActivityLog.EditRecommendedList"), recommendedList.Name), recommendedList);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.RecommendedLists.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = recommendedList.Id });
            }

            //prepare model
            model = _recommendedListModelFactory.PrepareRecommendedListModel(model, recommendedList, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //try to get a RecommendedList with the specified id
            var recommendedList = _recommendedListService.GetRecommendedListById(id);
            if (recommendedList == null)
                return RedirectToAction("List");

            _recommendedListService.DeleteRecommendedList(recommendedList);

            //activity log
            _customerActivityService.InsertActivity("DeleteRecommendedList",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteRecommendedList"), recommendedList.Name), recommendedList);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.RecommendedLists.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var recommendedLists = _recommendedListService.GetRecommendedListsByIds(selectedIds.ToArray());
                _recommendedListService.DeleteRecommendedLists(recommendedLists);

                recommendedLists.ForEach(recommendedList =>
                {
                    //activity log
                    _customerActivityService.InsertActivity("DeleteRecommendedList",
                        string.Format(_localizationService.GetResource("ActivityLog.DeleteRecommendedList"), recommendedList.Name), recommendedList);
                });
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Products

        [HttpPost]
        public virtual IActionResult ProductList(RecommendedListProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedDataTablesJson();

            //try to get a RecommendedList with the specified id
            var recommendedList = _recommendedListService.GetRecommendedListById(searchModel.RecommendedListId)
                ?? throw new ArgumentException("No RecommendedList found with the specified id");

            //prepare model
            var model = _recommendedListModelFactory.PrepareRecommendedListProductListModel(searchModel, recommendedList);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProductUpdate(RecommendedListProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //try to get a product RecommendedList with the specified id
            var productRecommendedList = _recommendedListService.GetProductRecommendedListById(model.Id)
                ?? throw new ArgumentException("No product RecommendedList mapping found with the specified id");

            //fill entity from model
            productRecommendedList = model.ToEntity(productRecommendedList);
            _recommendedListService.UpdateProductRecommendedList(productRecommendedList);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //try to get a product RecommendedList with the specified id
            var productRecommendedList = _recommendedListService.GetProductRecommendedListById(id)
                ?? throw new ArgumentException("No product RecommendedList mapping found with the specified id");

            _recommendedListService.DeleteProductRecommendedList(productRecommendedList);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAddPopup(int recommendedListId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //prepare model
            var model = _recommendedListModelFactory.PrepareAddProductToRecommendedListSearchModel(new AddProductToRecommendedListSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(AddProductToRecommendedListSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _recommendedListModelFactory.PrepareAddProductToRecommendedListListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(AddProductToRecommendedListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecommendedLists))
                return AccessDeniedView();

            //get selected products
            var selectedProducts = _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingProductRecommendedLists = _recommendedListService.GetProductRecommendedListsByRecommendedListId(model.RecommendedListId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //whether product RecommendedList with such parameters already exists
                    if (_recommendedListService.FindProductRecommendedList(existingProductRecommendedLists, product.Id, model.RecommendedListId) != null)
                        continue;

                    //insert the new product RecommendedList mapping
                    _recommendedListService.InsertProductRecommendedList(new ProductRecommendedList
                    {
                        RecommendedListId = model.RecommendedListId,
                        ProductId = product.Id,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToRecommendedListSearchModel());
        }

        #endregion
    }
}