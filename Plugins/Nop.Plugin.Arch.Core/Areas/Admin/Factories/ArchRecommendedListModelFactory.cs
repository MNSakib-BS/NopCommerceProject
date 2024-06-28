using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arch.Core.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the RecommendedList model factory implementation
    /// </summary>
    public partial class ArchRecommendedListModelFactory : RecommendedListModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IRecommendedListService _recommendedListService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly bool _hasMultipleLanguages;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ArchRecommendedListModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IRecommendedListService recommendedListService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IProductService productService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreContext storeContext) 
            : base( catalogSettings,
             baseAdminModelFactory,
             recommendedListService,
             localizationService,
             localizedModelFactory,
             productService,
             storeMappingSupportedModelFactory,
             storeContext)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _recommendedListService = recommendedListService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _productService = productService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeContext = storeContext;

            _hasMultipleLanguages = EngineContext.Current.Resolve<ILanguageService>()?.GetAllLanguages()?.Count > 1;

            _settingService = EngineContext.Current.Resolve<ISettingService>();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare RecommendedList product search model
        /// </summary>
        /// <param name="searchModel">RecommendedList product search model</param>
        /// <param name="recommendedList">RecommendedList</param>
        /// <returns>RecommendedList product search model</returns>
        protected override RecommendedListProductSearchModel PrepareRecommendedListProductSearchModel(RecommendedListProductSearchModel searchModel,
            RecommendedList recommendedList)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (recommendedList == null)
                throw new ArgumentNullException(nameof(RecommendedList));

            searchModel.RecommendedListId = recommendedList.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare RecommendedList search model
        /// </summary>
        /// <param name="searchModel">RecommendedList search model</param>
        /// <returns>RecommendedList search model</returns>
        public override RecommendedListSearchModel PrepareRecommendedListSearchModel(RecommendedListSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Catalog.RecommendedLists.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Catalog.RecommendedLists.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Catalog.RecommendedLists.List.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged RecommendedList list model
        /// </summary>
        /// <param name="searchModel">RecommendedList search model</param>
        /// <returns>RecommendedList list model</returns>
        public override RecommendedListListModel PrepareRecommendedListListModel(RecommendedListSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get RecommendedLists
            var recommendedLists = _recommendedListService.GetAllRecommendedLists(showHidden: true,
                recommendedListName: searchModel.SearchRecommendedListName,
                storeId: _storeContext.ActiveStoreScopeConfiguration,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));

            //prepare grid model
            var model = new RecommendedListListModel().PrepareToGrid(searchModel, recommendedLists, () =>
            {
                //fill in model values from the entity
                return recommendedLists.Select(recommendedList =>
                {
                    var recommendedListModel = recommendedList.ToModel<RecommendedListModel>();
                    return recommendedListModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare RecommendedList model
        /// </summary>
        /// <param name="model">RecommendedList model</param>
        /// <param name="recommendedList">RecommendedList</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>RecommendedList model</returns>
        public override RecommendedListModel PrepareRecommendedListModel(RecommendedListModel model,
            RecommendedList recommendedList, bool excludeProperties = false)
        {
            if (recommendedList != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = recommendedList.ToModel<RecommendedListModel>();
                }

                //prepare nested search model
                PrepareRecommendedListProductSearchModel(model.RecommendedListProductSearchModel, recommendedList);
            }

            //set default values for the new model
            if (recommendedList == null)
            {
                model.Published = false;
            }

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, recommendedList, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged RecommendedList product list model
        /// </summary>
        /// <param name="searchModel">RecommendedList product search model</param>
        /// <param name="recommendedList">RecommendedList</param>
        /// <returns>RecommendedList product list model</returns>
        public override RecommendedListProductListModel PrepareRecommendedListProductListModel(RecommendedListProductSearchModel searchModel,
            RecommendedList recommendedList)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (recommendedList == null)
                throw new ArgumentNullException(nameof(RecommendedList));

            //get product RecommendedLists
            var productRecommendedLists = _recommendedListService.GetProductRecommendedListsByRecommendedListId(showHidden: true,
                recommendedListId: recommendedList.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new RecommendedListProductListModel().PrepareToGrid(searchModel, productRecommendedLists, () =>
            {
                return productRecommendedLists.Select(productRecommendedList =>
                {
                    //fill in model values from the entity
                    var recommendedListProductModel = productRecommendedList.ToModel<RecommendedListProductModel>();

                    //fill in additional values (not existing in the entity)
                    recommendedListProductModel.ProductName = _productService.GetProductById(productRecommendedList.ProductId)?.Name;

                    return recommendedListProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged RecommendedList product list model
        /// </summary>
        /// <param name="searchModel">RecommendedList product search model</param>
        /// <param name="RecommendedList">RecommendedList</param>
        /// <returns>RecommendedList product list model</returns>
        public override RecommendedListProductListModel PrepareRecommendedListProductListModel(RecommendedListModel recommendedListModel)
        {
            if (recommendedListModel == null)
                throw new ArgumentNullException(nameof(RecommendedList));

            var searchModel = recommendedListModel.RecommendedListProductSearchModel;
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get product RecommendedLists
            var productRecommendedLists = _recommendedListService.GetProductRecommendedListsByRecommendedListId(showHidden: true,
                recommendedListId: recommendedListModel.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new RecommendedListProductListModel().PrepareToGrid(searchModel, productRecommendedLists, () =>
            {
                return productRecommendedLists.Select(productRecommendedList =>
                {
                    //fill in model values from the entity
                    var recommendedListProductModel = productRecommendedList.ToModel<RecommendedListProductModel>();

                    //fill in additional values (not existing in the entity)
                    recommendedListProductModel.ProductName = _productService.GetProductById(productRecommendedList.ProductId)?.Name;

                    return recommendedListProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare product search model to add to the RecommendedList
        /// </summary>
        /// <param name="searchModel">Product search model to add to the RecommendedList</param>
        /// <returns>Product search model to add to the RecommendedList</returns>
        public override AddProductToRecommendedListSearchModel PrepareAddProductToRecommendedListSearchModel(AddProductToRecommendedListSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available RecommendedLists
            _baseAdminModelFactory.PrepareRecommendedLists(searchModel.AvailableRecommendedLists);

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare available product types
            _baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product list model to add to the RecommendedList
        /// </summary>
        /// <param name="searchModel">Product search model to add to the RecommendedList</param>
        /// <returns>Product list model to add to the RecommendedList</returns>
        public override AddProductToRecommendedListListModel PrepareAddProductToRecommendedListListModel(AddProductToRecommendedListSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                //recommendedListId: searchModel.SearchRecommendedListId,
                storeId: _storeContext.ActiveStoreScopeConfiguration,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddProductToRecommendedListListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    return productModel;
                });
            });

            return model;
        }

        #endregion
    }
}