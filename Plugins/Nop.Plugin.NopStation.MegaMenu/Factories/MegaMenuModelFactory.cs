using Nop.Plugin.NopStation.MegaMenu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Plugin.NopStation.MegaMenu.Infrastructure.Cache;
using Nop.Plugin.NopStation.MegaMenu.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Services.Caching;
using Nop.Services.Customers;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Core.Domain.Stores;
using Nop.Services.Stores;
using Nop.Plugin.Arch.Core.Services.Catalogs;
using ArchNopCatalogDefaults = Nop.Plugin.Arch.Core.Services.Catalogs.NopCatalogDefaults;
using Nop.Plugin.Arch.Core.Services.Stores;

namespace Nop.Plugin.NopStation.MegaMenu.Factories
{
    public class MegaMenuModelFactory : IMegaMenuModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly ICategoryIconService _categoryIconService;
        private readonly CatalogSettings _catalogSettings;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly MegaMenuSettings _megaMenuSettings;
        private readonly IMegaMenuCoreService _megaMenuCoreService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<StoreTypeMapping> _storeTypeMappingRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IArchStoreProductInfoService _archStoreProductInfoService;
        private readonly IStoreMappingAdditionalService _storeMappingAdditionalService;

        private IEnumerable<CategoryProductCountModel> CategoryProductCounts { get; set; }
        #endregion

        #region Ctor

        public MegaMenuModelFactory(BlogSettings blogSettings,
            ICategoryIconService categoryIconService,
            CatalogSettings catalogSettings,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            MegaMenuSettings megaMenuSettings,
            IMegaMenuCoreService megaMenuCoreService,
            ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Category> categoryRepository,
            IRepository<StoreTypeMapping> storeTypeMappingRepository,
            IRepository<Product> productRepository,
            IArchStoreProductInfoService archStoreProductInfoService,
            IStoreMappingAdditionalService storeMappingAdditionalService
            )
        {
            _blogSettings = blogSettings;
            _categoryIconService = categoryIconService;
            _catalogSettings = catalogSettings;
            _displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _productService = productService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _megaMenuSettings = megaMenuSettings;
            _megaMenuCoreService = megaMenuCoreService;
            _cacheKeyService = cacheKeyService;
            _customerService = customerService;
            _productCategoryRepository = productCategoryRepository;
            _categoryRepository = categoryRepository;
            _storeTypeMappingRepository = storeTypeMappingRepository;
            _productRepository = productRepository;
            _archStoreProductInfoService = archStoreProductInfoService;
            _storeMappingAdditionalService = storeMappingAdditionalService;
        }

        #endregion
        protected virtual async Task<List<MegaMenuModel.TopicModel>> PrepareTopicMenuModelsAsync()
        {
            //top menu topics
            var topicCacheKey = _cacheManager.PrepareKeyForDefaultCache(MegaMenuModelCacheEventConsumer.MEGAMENU_TOPICS_MODEL_KEY,
                await _workContext.GetWorkingLanguageAsync(),
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                await _storeContext.GetCurrentStoreAsync());
            var cachedTopicModel = await _cacheManager.GetAsync(topicCacheKey, async () =>
                (await _topicService.GetAllTopicsAsync((await _storeContext.GetCurrentStoreAsync()).Id))
                .Where(t => t.IncludeInTopMenu)
                .SelectAwait(async t => new MegaMenuModel.TopicModel
                {
                    Id = t.Id,
                    Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
                    SeName = await _urlRecordService.GetSeNameAsync(t)
                })
                .ToListAsync()
            );

            return await cachedTopicModel;
        }

        protected virtual async Task<List<CategoryMenuModel>> PrepareCategoryMenuModelsAsync()
        {
            var loadPicture = _megaMenuSettings.ShowCategoryPicture;
            //load and cache them
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(MegaMenuModelCacheEventConsumer.MEGAMENU_CATEGORIES_MODEL_KEY,
                await _workContext.GetWorkingLanguageAsync(),
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
               await _storeContext.GetCurrentStoreAsync());

            var categoryMenuModel = await _cacheManager.GetAsync(cacheKey, async () =>
            {
                var ids = new List<int>();
                if (!string.IsNullOrWhiteSpace(_megaMenuSettings.SelectedCategoryIds))
                    ids = _megaMenuSettings.SelectedCategoryIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                CategoryProductCounts = await GetNumberOfProductsInCategoriesAsync();
                var allCategories = await _megaMenuCoreService.GetCategoriesByIdsAsync(_storeContext.GetCurrentStoreAsync().Id, ids, storeTypeId: _storeContext.StoreTypeId);
                return await PrepareCategoryMenuModelsAsync(allCategories, 0, loadPicture, skipItems: true);
            });

            return categoryMenuModel;
        }

        private async Task<IEnumerable<CategoryProductCountModel>> GetNumberOfProductsInCategoriesAsync()
        {
            var storeTypeId = 1;
            var storeTypeMapping = (await _storeTypeMappingRepository.Table.FirstOrDefaultAsync(p => p.StoreId == _storeContext.GetCurrentStoreAsync().Id));
            if (storeTypeMapping != null)
            {
                storeTypeId = storeTypeMapping.StoreTypeId;
            }

            var query = _productRepository.Table;

            var productCatQuery = from category in _categoryRepository.Table.Where(p => p.StoreTypeId == storeTypeId && !p.Deleted)
                                  join productCategory_tmp in _productCategoryRepository.Table on category.Id equals productCategory_tmp.CategoryId into productCategory_left
                                  from productCategory in productCategory_left.DefaultIfEmpty()
                                  select category.Id;

            query = from p in query
                    select p;

            query = query.Where(p => !p.Deleted && p.VisibleIndividually);

            query = await _archStoreProductInfoService.FilterProductsAsync(query, _storeContext.GetCurrentStoreAsync().Id);

            query =await _storeMappingAdditionalService.FilterStoresAsync(query, _storeContext.GetCurrentStoreAsync().Id);

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ArchNopCatalogDefaults.CategoryTotalNumberOfProductsCacheKey, _storeContext.GetCurrentStoreAsync().Id);

            var productCategoryMappingQuery = from pcm in _productCategoryRepository.Table
                                              select pcm;

            productCategoryMappingQuery =  await _archStoreProductInfoService.FilterProductsAsync(productCategoryMappingQuery, _storeContext.GetCurrentStoreAsync().Id);

            var queryResult = from product in query
                              join pcm in productCategoryMappingQuery on product.Id equals pcm.ProductId
                              join category in _categoryRepository.Table on pcm.CategoryId equals category.Id
                              where productCatQuery.Contains(pcm.CategoryId)
                              select new
                              {
                                  CategoryId = category.Id,
                                  ProductId = product.Id
                              };
            var finalQuery = from item in queryResult
                             group item by item.CategoryId into g
                             select new CategoryProductCountModel
                             {
                                 CategoryId = g.Key,
                                 ProductCount = g.Count()
                             };

            //only distinct products
            var result = await _cacheManager.GetAsync(cacheKey, async () => await finalQuery.ToListAsync());

            return result;
        }

        private class CategoryProductCountModel
        {
            public int CategoryId { get; set; }
            public int ProductCount { get; set; }
        }

        private async Task<List<CategoryMenuModel>> PrepareCategoryMenuModelsAsync(IEnumerable<Category> allCategories,
            int rootCategoryId, bool loadPicture, int level = 0, bool skipItems = false)
        {
            var result = new List<CategoryMenuModel>();

            var categories = allCategories as Category[] ?? allCategories.ToArray();
            var rootCategories = categories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();

            foreach (var category in rootCategories)
            {
                var catModel = new CategoryMenuModel
                {
                    Id = category.Id,
                    Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(category),
                    NumberOfProducts = 0
                };

                if (loadPicture)
                {
                    var categoryIcon = await _categoryIconService.GetCategoryIconByCategoryIdAsync(category.Id);
                    if (categoryIcon != null)
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(categoryIcon.PictureId);
                        if (picture != null)
                        {
                            catModel.PictureModel = new PictureModel
                            {
                                ImageUrl = (await _pictureService.GetPictureUrlAsync(picture)).ToString(),
                                FullSizeImageUrl = ( await _pictureService.GetPictureUrlAsync(picture, 0, false)).ToString(),
                                Title = string.Format(await _localizationService.GetResourceAsync("Media.Category.ImageLinkTitleFormat"), catModel.Name),
                                AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Category.ImageAlternateTextFormat"), catModel.Name)
                            };
                        }
                    }
                }

                if (!skipItems)
                {
                    var subCategories = await PrepareCategoryMenuModelsAsync(categories, category.Id, loadPicture, level + 1);
                    catModel.SubCategories.AddRange(subCategories);
                }

                if (CategoryProductCounts != null)
                {
                    var categoryProductCount = CategoryProductCounts.FirstOrDefault(p => p.CategoryId == category.Id);
                    if (categoryProductCount != null)
                        catModel.NumberOfProducts = categoryProductCount.ProductCount;
                }

                result.Add(catModel);
            }

            return result;
        }

        public async Task<MegaMenuModel> PrepareMegaMenuModelAsync()
        {
            var cachedMegaMenuModel = new MegaMenuModel
            {
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                DisplayHomePageMenuItem = _displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                DisplayNewProductsMenuItem = _displayDefaultMenuItemSettings.DisplayNewProductsMenuItem,
                DisplayProductSearchMenuItem = _displayDefaultMenuItemSettings.DisplayProductSearchMenuItem,
                DisplayCustomerInfoMenuItem = _displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem,
                DisplayBlogMenuItem = _displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                DisplayForumsMenuItem = _displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem,
                MaxCategoryLevelsToShow = _megaMenuSettings.MaxCategoryLevelsToShow,
                HideManufacturers = _megaMenuSettings.HideManufacturers,
                DisplayPromotionsMenuItem = _displayDefaultMenuItemSettings.DisplayPromotionsMenuItem
            };

            var categoryMenuModels = await PrepareCategoryMenuModelsAsync();
            var topicMenuModels = await PrepareTopicMenuModelsAsync();

            cachedMegaMenuModel.Categories = categoryMenuModels;
            cachedMegaMenuModel.Topics = topicMenuModels;

            return cachedMegaMenuModel;
        }
    }
}
