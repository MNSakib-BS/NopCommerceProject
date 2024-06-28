using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Services.Stores;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class CategoryAdditionalService : ICategoryAdditionalService
{
    #region Fields

    private readonly IRepository<CategoryAdditional> _categoryAdditionalRepository;
    private readonly IRepository<PromotedProductCategory> _promotedProductCategoryRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICategoryService _categoryService;
    private readonly IRepository<Product> _productRepository;
    private readonly CatalogSettings _catalogSettings;
    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<AclRecord> _aclRepository;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreContextAdditional _storeContextAdditional;
    private readonly IArchStoreProductInfoService _archStoreProductInfoService;
    private readonly IRepository<StoreTypeMapping> _storeTypeMappingRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IStaticCacheManager _cacheManager;
    private readonly ICacheKeyService _cacheKeyService;
    private readonly IRepository<ProductCategoryAdditional> _productCategoryAdditionalRepository;


    #endregion

    #region Ctor

    public CategoryAdditionalService(IRepository<CategoryAdditional> categoryAdditionalRepository,
        IRepository<PromotedProductCategory> promotedProductCategoryRepository,
        IEventPublisher eventPublisher,
        ICategoryService categoryService,
        IRepository<Product> productRepository,
        CatalogSettings catalogSettings,
        ICustomerService customerService,
        IWorkContext workContext,
        IRepository<Category> categoryRepository,
        IRepository<AclRecord> aclRepository,
        IStoreMappingService storeMappingService,
        IStoreContext storeContext,
        IStoreContextAdditional storeContextAdditional,
        IArchStoreProductInfoService archStoreProductInfoService,
        IRepository<StoreTypeMapping> storeTypeMappingRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IStaticCacheManager cacheManager,
        ICacheKeyService cacheKeyService,
        IRepository<ProductCategoryAdditional> productCategoryAdditionalRepository)
    {
        _categoryAdditionalRepository = categoryAdditionalRepository;
        _promotedProductCategoryRepository = promotedProductCategoryRepository;
        _eventPublisher = eventPublisher;
        _categoryService = categoryService;
        _productRepository = productRepository;
        _catalogSettings = catalogSettings;
        _customerService = customerService;
        _workContext = workContext;
        _categoryRepository = categoryRepository;
        _aclRepository = aclRepository;
        _storeMappingService = storeMappingService;
        _storeContext = storeContext;
        _storeContextAdditional = storeContextAdditional;
        _archStoreProductInfoService = archStoreProductInfoService;
        _storeTypeMappingRepository = storeTypeMappingRepository;
        _productCategoryRepository = productCategoryRepository;
        _cacheManager = cacheManager;
        _cacheKeyService = cacheKeyService;
        _productCategoryAdditionalRepository = productCategoryAdditionalRepository;
    }

    #endregion

    #region Utilities

    private async Task<IQueryable<PromotedProductCategory>> GetPromotedProductCategoriesByCategoryIdQueryAsync(int categoryId, bool showHidden = false)
    {
        var query = from pc in _promotedProductCategoryRepository.Table
                    join p in _productRepository.Table on pc.ProductId equals p.Id
                    where pc.CategoryId == categoryId &&
                          !p.Deleted &&
                          (showHidden || p.Published)
                    orderby pc.DisplayOrder, pc.Id
                    select pc;

        if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
        {
            if (!_catalogSettings.IgnoreAcl)
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                //ACL (access control list)
                var allowedCustomerRolesIds = await _customerService.GetCustomerRoleIdsAsync(customer);
                query = from pc in query
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        join acl in _aclRepository.Table
                            on new
                            {
                                c1 = c.Id,
                                c2 = nameof(Category)
                            }
                            equals new
                            {
                                c1 = acl.EntityId,
                                c2 = acl.EntityName
                            }
                            into c_acl
                        from acl in c_acl.DefaultIfEmpty()
                        where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select pc;
            }

            var store=await _storeContext.GetCurrentStoreAsync();

            query = await _storeMappingService.ApplyStoreMapping(query, store.Id);

            query = query.Distinct().OrderBy(pc => pc.DisplayOrder).ThenBy(pc => pc.Id);
        }

        return query;
    }


    #endregion

    #region Methods

    #region Category Additional Domain

    public virtual async Task DeleteCategoryAdditionalAsync(CategoryAdditional categoryAdditional)
    {
        await _categoryAdditionalRepository.DeleteAsync(categoryAdditional);
    }

    public virtual async Task DeletePromotedProductCategoryAsync(PromotedProductCategory promotedProductCategory)
    {
        if (promotedProductCategory == null)
            throw new ArgumentNullException(nameof(promotedProductCategory));

       await _promotedProductCategoryRepository.DeleteAsync(promotedProductCategory);

        //event notification
       await _eventPublisher.EntityDeletedAsync(promotedProductCategory);
    }

    public virtual Task<PromotedProductCategory> FindPromotedProductCategoryAsync(IList<PromotedProductCategory> source, int productId, int categoryId)
    {
        PromotedProductCategory result = null;

        foreach (var promotedProductCategory in source)
        {
            if (promotedProductCategory.ProductId == productId && promotedProductCategory.CategoryId == categoryId)
            {
                result = promotedProductCategory;
                break;
            }
        }

        return Task.FromResult(result);
    }


    public virtual async Task<IList<Category>> GetAllCategoriesByTypeAsync(CategoryType type, int storeId = 0)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(storeId);

        return await(from category in categories
                     join ca in _categoryAdditionalRepository.Table on category.Id equals ca.CategoryId
                     where ca.CategoryType == type
                     select category).ToListAsync();

    }

    public virtual async Task<CategoryAdditional> GetCategoryAdditionalByCategoryIdAsync(int categoryId)
    {
        return await _categoryAdditionalRepository.Table.Where(e => e.CategoryId == categoryId).FirstOrDefaultAsync();
    }

    public virtual async Task<CategoryAdditional> GetCategoryAdditionalByIdAsync(int id)
    {
        return await _categoryAdditionalRepository.GetByIdAsync(id, cache => default, false);
    }

    public virtual async Task<Category> GetCategoryByExternalIdAsync(int externalId, CategoryType categoryType = CategoryType.Default, int? externalParentId = null, int storeId = 0, int storeTypeId = 0)
    {
        if (storeTypeId == 0 && storeId == 0)
            storeTypeId = _storeContextAdditional.StoreTypeId;

        var categoryTypeId = (int)categoryType;

        IQueryable<Category> query = null;
        if (externalParentId.HasValue)
        {
            query = from c in _categoryRepository.Table
                    join ca in _categoryAdditionalRepository.Table on c.Id equals ca.CategoryId
                    where ca.ExternalId == externalId &&
                    ca.ExternalParentId == externalParentId &&
                    ca.CategoryTypeId == categoryTypeId
                    select c;
        }
        else
        {
            query = from c in _categoryRepository.Table
                    join ca in _categoryAdditionalRepository.Table on c.Id equals ca.CategoryId
                    where ca.ExternalId == externalId &&
                          ca.CategoryTypeId == categoryTypeId
                    select c;
        }

        if (storeTypeId > 0)
        {
            query =await  _archStoreProductInfoService.FilterCategoriesAsync(query, storeTypeId: storeTypeId);
        }
        else
            query = await _archStoreProductInfoService.FilterCategoriesAsync(query, storeId: storeId);
        query = query.Where(p => !p.Deleted);
        return query.FirstOrDefault();
    }

    public async Task<Category> GetCategoryByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        return await _categoryRepository.Table.FirstOrDefaultAsync(i => i.Name == name);
    }

    public virtual async Task<IEnumerable<int>> GetNumberOfProductsInCategoriesAsync()
    {
        var storeTypeId = 1;
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        var storeTypeMapping = await  _storeTypeMappingRepository.Table.FirstOrDefaultAsync(p => p.StoreId == currentStore.Id);
        if (storeTypeMapping != null)
        {
            storeTypeId = storeTypeMapping.StoreTypeId;
        }

        var query = _productRepository.Table;

        var productCatQuery = from category in _categoryRepository.Table.Where(p => !p.Deleted)
                              join ca in _categoryAdditionalRepository.Table.Where(p=> p.StoreTypeId == storeTypeId) on category.Id equals ca.CategoryId
                              join productCategory_tmp in _productCategoryRepository.Table on category.Id equals productCategory_tmp.CategoryId into productCategory_left
                              from productCategory in productCategory_left.DefaultIfEmpty()
                              select category.Id;

        query = from p in query
                join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                where productCatQuery.Contains(pc.CategoryId)
                select p;

        query = query.Where(p => !p.Deleted && p.VisibleIndividually);

        query = await _archStoreProductInfoService.FilterProductsAsync(query, currentStore.Id);

        query = await _storeMappingService.ApplyStoreMapping(query, currentStore.Id);

        var cacheKey =  _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryTotalNumberOfProductsCacheKey, currentStore.Id);

        var productCategoryMappingQuery = from pcm in _productCategoryRepository.Table
                                          select pcm;

        // productCategoryMappingQuery = _storeMappingService.FilterStores(productCategoryMappingQuery, _storeContext.CurrentStore.Id);

        var queryResult = from product in query
                          join pcm in productCategoryMappingQuery on product.Id equals pcm.ProductId
                          join category in _categoryRepository.Table on pcm.CategoryId equals category.Id
                          select new
                          {
                              CategoryId = category.Id,
                              ProductId = product.Id
                          };

        var finalQuery = from item in queryResult
                         group item by item.CategoryId into g
                         select g.Key;

        // only distinct products
        var result = await _cacheManager.GetAsync(cacheKey, async () => await finalQuery.ToListAsync());

        return result;
    }


    public virtual async Task<ProductCategory> GetProductCategoryByIdsAsync(int productId, int categoryId, int storeId = 0)
    {
        var query = _productCategoryRepository.Table.Where(i => i.ProductId == productId && i.CategoryId == categoryId);

        var productCategoryAdditionalQuery = from pca in _productCategoryAdditionalRepository.Table
                                             join pc in query on pca.ProductCategoryId equals pc.Id
                                             select pca;

        productCategoryAdditionalQuery = await _storeMappingService.ApplyStoreMapping(productCategoryAdditionalQuery, storeId);

        return await (from pc in query
               join pca in productCategoryAdditionalQuery on pc.Id equals pca.ProductCategoryId
               select pc).FirstOrDefaultAsync();
        
    }
   

    /// <summary>
    /// Gets promoted product category mapping collection
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>Product a category mapping collection</returns>

    public virtual async Task<IPagedList<PromotedProductCategory>> GetPromotedProductCategoriesByCategoryIdAsync(int categoryId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
    {
        if (categoryId == 0)
            return new PagedList<PromotedProductCategory>(new List<PromotedProductCategory>(), pageIndex, pageSize);

        var query = await GetPromotedProductCategoriesByCategoryIdQueryAsync(categoryId, showHidden);           

        return await query.ToPagedListAsync(pageIndex, pageSize);
    } 


    /// <summary>
    /// Gets a promoted product category mapping 
    /// </summary>
    /// <param name="promotedProductCategoryId">Promoted product category mapping identifier</param>
    /// <returns>Promoted product category mapping</returns>
    public virtual async Task<PromotedProductCategory> GetPromotedProductCategoryByIdAsync(int promotedProductCategoryId)
    {
        if (promotedProductCategoryId == 0)
            return null;

        return await _promotedProductCategoryRepository.GetByIdAsync(promotedProductCategoryId, cache => default, false);
    }

    public virtual async Task<IList<int>> GetPromotedProductIDsByCategoryIdAsync(int categoryId, bool showHidden = false)
    {
        if (categoryId == 0)
            return new List<int>();

        var query = await GetPromotedProductCategoriesByCategoryIdQueryAsync(categoryId, showHidden);

        var productCategories = query.Select(s => s.ProductId).ToList();

        return productCategories;
    }

    public virtual async Task<Category> GetRootCategoryAsync(int productId)
    {
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);

        foreach (var item in productCategories)
        {
            var category = await _categoryService.GetCategoryByIdAsync(item.CategoryId);

            if (category.ParentCategoryId == 0)
                return category;
        }

        return null;
    }

    public virtual async Task InsertCategoryAdditionalAsync(CategoryAdditional categoryAdditional)
    {
        await _categoryAdditionalRepository.InsertAsync(categoryAdditional);
    }

    /// <summary>
    /// Inserts a promoted product category mapping
    /// </summary>
    /// <param name="promotedProductCategory">>Promoted product category mapping</param>
    public virtual async Task InsertPromotedProductCategoryAsync(PromotedProductCategory promotedProductCategory)
    {
        if (promotedProductCategory == null)
            throw new ArgumentNullException(nameof(promotedProductCategory));

        await _promotedProductCategoryRepository.InsertAsync(promotedProductCategory);

        //event notification
       await _eventPublisher.EntityInsertedAsync(promotedProductCategory);
    }

    public virtual async Task UpdateCategoryAdditionalAsync(CategoryAdditional categoryAdditional)
    {
        await _categoryAdditionalRepository.UpdateAsync(categoryAdditional);
    }

    /// <summary>
    /// Updates the promoted product category mapping 
    /// </summary>
    /// <param name="promotedProductCategory">>Promoted product category mapping</param>
    public virtual async Task UpdatePromotedProductCategoryAsync(PromotedProductCategory promotedProductCategory)
    {
        if (promotedProductCategory == null)
            throw new ArgumentNullException(nameof(promotedProductCategory));

        await _promotedProductCategoryRepository.UpdateAsync(promotedProductCategory);
        //event notification
        await _eventPublisher.EntityUpdatedAsync(promotedProductCategory);
    }

    #endregion

    #region Custom methods from Category




    #endregion


    #endregion

}
