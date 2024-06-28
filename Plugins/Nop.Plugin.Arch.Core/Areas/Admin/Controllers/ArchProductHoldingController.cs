using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Arch.Core.Domains.Api;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Arch.Core.Areas.Admin.Controllers
{
    public partial class ArchProductHoldingController : BaseAdminController
    {
        private readonly ArchApiSettings _archSettings;
        private readonly ISettingService _settingService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWorkContext _workContext;

        public ArchProductHoldingController(
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _workContext = workContext;

            _settingService = EngineContext.Current.Resolve<ISettingService>();
            _archSettings = _settingService.LoadSetting<ArchApiSettings>(storeContext.ActiveStoreScopeConfiguration);
        }

        protected virtual async Task SaveCategoryMappings(Product product, ProductModel model)
        {
            var existingProductCategories =await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                   await _categoryService.DeleteProductCategoryAsync(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                if (_categoryService.FindProductCategory(existingProductCategories, product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping =await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    await _categoryService.InsertProductCategoryAsync(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model =await _productModelFactory.PrepareProductSearchModelAsync(new ProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(ProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model =await _productModelFactory.PrepareProductListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product =await _productService.GetProductByIdAsync(id);
            if (product == null || product.Deleted)
                return RedirectToAction("List");


            var currentVendor=await _workContext.GetCurrentVendorAsync();

            //a vendor should have access only to his products
            if (currentVendor != null && product.VendorId !=currentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model =await _productModelFactory.PrepareProductModelAsync(null, product);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ProductModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product =await _productService.GetProductByIdAsync(model.Id);
            if (product == null || product.Deleted)
                return RedirectToAction("List");

            var currentVendor = await _workContext.GetCurrentVendorAsync();

            //a vendor should have access only to his products
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (currentVendor != null)
                    model.VendorId = currentVendor.Id;

                //update product from form
                product.Name = model.Name;
                product.ShortDescription = model.ShortDescription;
                product.FullDescription = model.FullDescription;
                product.Sku = model.Sku;
                product.POSItemField = model.POSItemField;
                product.TradeOnlineField = model.TradeOnlineField;
                product.UpdatedOnUtc = DateTime.UtcNow;

                //Updates the categories
                await SaveCategoryMappings(product, model);

                _productService.CalculateIsInHolding(product);
               await _productService.UpdateProductAsync(product);

                //Updates the activity log
               await _customerActivityService.InsertActivityAsync("EditProduct",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProduct"), product.Name), product);

               _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //prepare model
            model =await _productModelFactory.PrepareProductModelAsync(model, product, true);

            //something went wrong, will redisplay the form
            return View(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-product-by-sku")]
        public virtual async Task<IActionResult> GoToSku(ProductSearchModel searchModel)
        {
            var product = await _productService.GetProductBySkuAsync(searchModel.GoDirectlyToSku);
            var productAttributeCombination = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(searchModel.GoDirectlyToSku);

            //try to load a product entity, if not found, then try to load a product attribute combination
            var productId = product?.Id
                ?? productAttributeCombination?.ProductId;

            if (productId != null)
                return RedirectToAction("Edit", "ProductHolding", new { id = productId });

            //not found
            return await List();
        }

        //action displaying notification (warning) to a store owner that entered SKU already exists
        public virtual async Task<IActionResult> SkuReservedWarning(int productId, string sku)
        {
            string message;

            //check whether product with passed SKU already exists
            var productBySku =await _productService.GetProductBySkuAsync(sku);
            if (productBySku != null)
            {
                if (productBySku.Id == productId)
                    return Json(new { Result = string.Empty });

                message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Sku.Reserved"), productBySku.Name);
                return Json(new { Result = message });
            }

            //check whether combination with passed SKU already exists
            var combinationBySku =await _productAttributeService.GetProductAttributeCombinationBySkuAsync(sku);
            if (combinationBySku == null)
                return Json(new { Result = string.Empty });

            var product = await _productService.GetProductByIdAsync(combinationBySku.ProductId);

            message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Reserved"),product?.Name);

            return Json(new { Result = message });
        }
    }
}
