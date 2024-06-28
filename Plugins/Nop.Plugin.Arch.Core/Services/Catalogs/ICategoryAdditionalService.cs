using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Arch.Core.Domains.Affiliates;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface ICategoryAdditionalService
{
   /// <summary>
   /// 
   /// </summary>
   /// <param name="categoryId"></param>
   /// <returns></returns>
    Task<CategoryAdditional> GetCategoryAdditionalByCategoryIdAsync(int categoryId);    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<CategoryAdditional> GetCategoryAdditionalByIdAsync(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="categoryAdditional"></param>
    /// <returns></returns>
    Task DeleteCategoryAdditionalAsync(CategoryAdditional categoryAdditional);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="categoryAdditional"></param>
    /// <returns></returns>
    Task InsertCategoryAdditionalAsync(CategoryAdditional categoryAdditional);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="categoryAdditional"></param>
    /// <returns></returns>
    Task UpdateCategoryAdditionalAsync(CategoryAdditional categoryAdditional);

    /// <summary>
    /// Gets all categories filtered by parent category identifier
    /// </summary>
    /// <param name="type">Category type</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <returns>Categories</returns>
    Task<IList<Category>> GetAllCategoriesByTypeAsync(CategoryType type, int storeId = 0);

    /// <summary>
    /// Gets a category
    /// </summary>
    /// <param name="categoryId">Category name</param>
    /// <returns>Category</returns>
    Task<Category> GetCategoryByNameAsync(string name);

    /// <summary>
    /// Gets a category
    /// </summary>
    /// <param name="externalId">External identifier</param>
    /// <param name="categoryType">Category type</param>
    /// <param name="externalParentId">External parent identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Category</returns>
    Task<Category> GetCategoryByExternalIdAsync(int externalId, CategoryType categoryType = CategoryType.Default, int? externalParentId = null, int storeId = 0, int storeTypeId = 0);

    /// <summary>
    /// Deletes a promoted product category mapping
    /// </summary>
    /// <param name="promotedProductCategory">Promoted product category</param>
    Task DeletePromotedProductCategoryAsync(PromotedProductCategory promotedProductCategory);

    /// <summary>
    /// Gets promoted product category mapping collection
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>Product a category mapping collection</returns>
    Task<IPagedList<PromotedProductCategory>> GetPromotedProductCategoriesByCategoryIdAsync(int categoryId,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

    /// <summary>
    /// Gets a promoted product category mapping 
    /// </summary>
    /// <param name="promotedProductCategoryId">Promoted product category mapping identifier</param>
    /// <returns>Promoted product category mapping</returns>
    Task<PromotedProductCategory> GetPromotedProductCategoryByIdAsync(int promotedProductCategoryId);

    /// <summary>
    /// Gets a product category mapping 
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Product category mapping</returns>
    Task<ProductCategory> GetProductCategoryByIdsAsync(int productId, int categoryId, int storeId = 0);

    /// <summary>
    /// Inserts a promoted product category mapping
    /// </summary>
    /// <param name="promotedProductCategory">>Promoted product category mapping</param>
    Task InsertPromotedProductCategoryAsync(PromotedProductCategory promotedProductCategory);

    /// Updates the promoted product category mapping 
    /// </summary>
    /// <param name="promotedProductCategory">>Promoted product category mapping</param>
    Task UpdatePromotedProductCategoryAsync(PromotedProductCategory promotedProductCategory);

    /// Returns a PromotedProductCategory that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="productId">Product identifier</param>
    /// <param name="categoryId">Category identifier</param>
    /// <returns>A PromotedProductCategory that has the specified values; otherwise null</returns>
    Task<PromotedProductCategory> FindPromotedProductCategoryAsync(IList<PromotedProductCategory> source, int productId, int categoryId);
    Task<Category> GetRootCategoryAsync(int productId);
    Task<IList<int>> GetPromotedProductIDsByCategoryIdAsync(int categoryId, bool showHidden = false);
    Task<IEnumerable<int>> GetNumberOfProductsInCategoriesAsync();
}
