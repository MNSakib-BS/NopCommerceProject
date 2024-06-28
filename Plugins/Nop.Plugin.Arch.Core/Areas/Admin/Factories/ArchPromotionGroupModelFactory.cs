using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;
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
    /// Represents the PromotionGroup model factory implementation
    /// </summary>
    public partial class ArchPromotionGroupModelFactory : PromotionGroupModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IPromotionGroupService _promotionGroupService;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;
        private readonly bool _hasMultipleLanguages; 
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ArchPromotionGroupModelFactory(CatalogSettings catalogSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            IPromotionGroupService promotionGroupService,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IProductService productService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext)
            : base( catalogSettings,
             aclSupportedModelFactory,
             baseAdminModelFactory,
             promotionGroupService,
             discountService,
             discountSupportedModelFactory,
             localizationService,
             localizedModelFactory,
             productService,
             storeMappingSupportedModelFactory,
             urlRecordService,
             storeContext)
        {
            _catalogSettings = catalogSettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _promotionGroupService = promotionGroupService;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _productService = productService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _urlRecordService = urlRecordService;
            _storeContext = storeContext;

            _hasMultipleLanguages = EngineContext.Current.Resolve<ILanguageService>()?.GetAllLanguages()?.Count > 1;

            _settingService = EngineContext.Current.Resolve<ISettingService>();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare PromotionGroup product search model
        /// </summary>
        /// <param name="searchModel">PromotionGroup product search model</param>
        /// <param name="PromotionGroup">PromotionGroup</param>
        /// <returns>PromotionGroup product search model</returns>
        protected override PromotionGroupProductSearchModel PreparePromotionGroupProductSearchModel(PromotionGroupProductSearchModel searchModel,
            PromotionGroup PromotionGroup)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (PromotionGroup == null)
                throw new ArgumentNullException(nameof(PromotionGroup));

            searchModel.PromotionGroupId = PromotionGroup.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare PromotionGroup search model
        /// </summary>
        /// <param name="searchModel">PromotionGroup search model</param>
        /// <returns>PromotionGroup search model</returns>
        public override PromotionGroupSearchModel PreparePromotionGroupSearchModel(PromotionGroupSearchModel searchModel)
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
                Text = _localizationService.GetResource("Admin.Promotions.PromotionGroups.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Promotions.PromotionGroups.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Promotions.PromotionGroups.List.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged PromotionGroup list model
        /// </summary>
        /// <param name="searchModel">PromotionGroup search model</param>
        /// <returns>PromotionGroup list model</returns>
        public override PromotionGroupListModel PreparePromotionGroupListModel(PromotionGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get PromotionGroups
            var PromotionGroups = _promotionGroupService.GetAllPromotionGroups(showHidden: true,
                PromotionGroupName: searchModel.SearchPromotionGroupName,
                storeId: _storeContext.ActiveStoreScopeConfiguration,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));

            //prepare grid model
            var model = new PromotionGroupListModel().PrepareToGrid(searchModel, PromotionGroups, () =>
            {
                //fill in model values from the entity
                return PromotionGroups.Select(PromotionGroup =>
                {
                    var PromotionGroupModel = PromotionGroup.ToModel<PromotionGroupModel>();
                    PromotionGroupModel.SeName = _urlRecordService.GetSeName(PromotionGroup, 0, true, false);

                    return PromotionGroupModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare PromotionGroup model
        /// </summary>
        /// <param name="model">PromotionGroup model</param>
        /// <param name="promotionGroup">PromotionGroup</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>PromotionGroup model</returns>
        public override PromotionGroupModel PreparePromotionGroupModel(PromotionGroupModel model,
            PromotionGroup promotionGroup, bool excludeProperties = false)
        {
            Action<PromotionGroupLocalizedModel, int> localizedModelConfiguration = null;

            if (promotionGroup != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = promotionGroup.ToModel<PromotionGroupModel>();
                    model.SeName = _urlRecordService.GetSeName(promotionGroup, 0, true, false);
                }

                //prepare nested search model
                PreparePromotionGroupProductSearchModel(model.PromotionGroupProductSearchModel, promotionGroup);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _hasMultipleLanguages ? _localizationService.GetLocalized(promotionGroup, entity => entity.Name, languageId, false, false) : promotionGroup.Name;
                    locale.Description = _hasMultipleLanguages ? _localizationService.GetLocalized(promotionGroup, entity => entity.Description, languageId, false, false) : promotionGroup.Description;
                    locale.MetaKeywords = _hasMultipleLanguages ? _localizationService.GetLocalized(promotionGroup, entity => entity.MetaKeywords, languageId, false, false) : promotionGroup.MetaKeywords;
                    locale.MetaDescription = _hasMultipleLanguages ? _localizationService.GetLocalized(promotionGroup, entity => entity.MetaDescription, languageId, false, false) : promotionGroup.MetaDescription;
                    locale.MetaTitle = _hasMultipleLanguages ? _localizationService.GetLocalized(promotionGroup, entity => entity.MetaTitle, languageId, false, false) : promotionGroup.MetaTitle;
                    locale.SeName = _urlRecordService.GetSeName(promotionGroup, languageId, false, false);
                };
            }

            //set default values for the new model
            if (promotionGroup == null)
            {
                model.PageSize = _catalogSettings.DefaultPromotionGroupPageSize;
                model.PageSizeOptions = _catalogSettings.DefaultPromotionGroupPageSizeOptions;
                model.Published = false;
                model.AllowCustomersToSelectPageSize = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare available PromotionGroup templates
            _baseAdminModelFactory.PreparePromotionGroupTemplates(model.AvailablePromotionGroupTemplates, false);

            //prepare model discounts
            var availableDiscounts = _discountService.GetAllDiscounts(_storeContext.ActiveStoreScopeConfiguration, DiscountType.AssignedToPromotionGroups, showHidden: true);
            _discountSupportedModelFactory.PrepareModelDiscounts(model, promotionGroup, availableDiscounts, excludeProperties);

            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model, promotionGroup, excludeProperties);

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, promotionGroup, excludeProperties);

            //banner widget zones
            PrepareWidgetZones(model, false);

            return model;
        }

        public PromotionGroupModel PrepareWidgetZones(PromotionGroupModel model, bool editing)
        {
            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.value, y => y.id);

            IList<int> currentWidgetZones;

            if (editing)
            {
                currentWidgetZones = model.WidgetZones;
            }
            else
            {
                currentWidgetZones = (from i in _promotionGroupService.GetWidgetZonesForPromotionGroup(model.Id)
                                      where lookup.ContainsKey(i)
                                      select lookup[i]).ToList();
            }

            var avaialbleWidgetZones = from widgetZone in widgetZonesData
                                       select new SelectListItem
                                       {
                                           Text = widgetZone.value,
                                           Value = widgetZone.id.ToString(),
                                           Selected = currentWidgetZones.Contains(widgetZone.id)
                                       };

            model.WidgetZones = currentWidgetZones.ToList();
            model.AvailableWidgetZones = avaialbleWidgetZones.ToList();

            return model;
        }

        private List<(string value, int id)> GetWidgetZoneData()
        {
            var widgetZones = _settingService.GetSettingByKey("promotionbannersettings.widgetzones", string.Empty, _storeContext.ActiveStoreScopeConfiguration);
            var allWidgetZones = widgetZones.Split(";").ToList();

            int id = 1000;
            return allWidgetZones.OrderBy(x => x)
                                 .Select(x => (value: x, id++))
                                 .ToList();
        }

        /// <summary>
        /// Prepare paged PromotionGroup product list model
        /// </summary>
        /// <param name="searchModel">PromotionGroup product search model</param>
        /// <param name="PromotionGroup">PromotionGroup</param>
        /// <returns>PromotionGroup product list model</returns>
        public override PromotionGroupProductListModel PreparePromotionGroupProductListModel(PromotionGroupProductSearchModel searchModel,
            PromotionGroup PromotionGroup)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (PromotionGroup == null)
                throw new ArgumentNullException(nameof(PromotionGroup));

            //get product PromotionGroups
            var productPromotionGroups = _promotionGroupService.GetProductPromotionGroupsByPromotionGroupId(showHidden: true,
                PromotionGroupId: PromotionGroup.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new PromotionGroupProductListModel().PrepareToGrid(searchModel, productPromotionGroups, () =>
            {
                return productPromotionGroups.Select(productPromotionGroup =>
                {
                    //fill in model values from the entity
                    var PromotionGroupProductModel = productPromotionGroup.ToModel<PromotionGroupProductModel>();

                    //fill in additional values (not existing in the entity)
                    PromotionGroupProductModel.ProductName = _productService.GetProductById(productPromotionGroup.ProductId)?.Name;

                    return PromotionGroupProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged PromotionGroup product list model
        /// </summary>
        /// <param name="searchModel">PromotionGroup product search model</param>
        /// <param name="PromotionGroup">PromotionGroup</param>
        /// <returns>PromotionGroup product list model</returns>
        public override PromotionGroupProductListModel PreparePromotionGroupProductListModel(PromotionGroupModel promotionGroupModel)
        {
            if (promotionGroupModel == null)
                throw new ArgumentNullException(nameof(PromotionGroup));

            var searchModel = promotionGroupModel.PromotionGroupProductSearchModel;
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get product PromotionGroups
            var productPromotionGroups = _promotionGroupService.GetProductPromotionGroupsByPromotionGroupId(showHidden: true,
                PromotionGroupId: promotionGroupModel.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new PromotionGroupProductListModel().PrepareToGrid(searchModel, productPromotionGroups, () =>
            {
                return productPromotionGroups.Select(productPromotionGroup =>
                {
                    //fill in model values from the entity
                    var PromotionGroupProductModel = productPromotionGroup.ToModel<PromotionGroupProductModel>();

                    //fill in additional values (not existing in the entity)
                    PromotionGroupProductModel.ProductName = _productService.GetProductById(productPromotionGroup.ProductId)?.Name;

                    return PromotionGroupProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare product search model to add to the PromotionGroup
        /// </summary>
        /// <param name="searchModel">Product search model to add to the PromotionGroup</param>
        /// <returns>Product search model to add to the PromotionGroup</returns>
        public override AddProductToPromotionGroupSearchModel PrepareAddProductToPromotionGroupSearchModel(AddProductToPromotionGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available PromotionGroups
            _baseAdminModelFactory.PreparePromotionGroups(searchModel.AvailablePromotionGroups);

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
        /// Prepare paged product list model to add to the PromotionGroup
        /// </summary>
        /// <param name="searchModel">Product search model to add to the PromotionGroup</param>
        /// <returns>Product list model to add to the PromotionGroup</returns>
        public override AddProductToPromotionGroupListModel PrepareAddProductToPromotionGroupListModel(AddProductToPromotionGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                //promotionGroupId: searchModel.SearchPromotionGroupId,
                storeId: _storeContext.ActiveStoreScopeConfiguration,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddProductToPromotionGroupListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        #endregion
    }
}