using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public static partial class NopCatalogDefaults
{
    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : product ID
    /// </remarks>
    public static string ProductPromotionGroupsByProductPrefixCacheKey => "Nop.productpromotiongroup.allbyproductid-{0}";

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : product id
    /// </remarks>
    public static string ProductPricePrefixCacheKey => "Nop.totals.productprice-{0}";

    #region PromotionGroup template

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    public static CacheKey PromotionGroupTemplatesAllCacheKey => new CacheKey("Nop.promotiongrouptemplate.all");

    public static CacheKey CategoryTotalNumberOfProductsCacheKey => new CacheKey("Nop.productcategory.totalnumberofproducts-{0}", CategoryTotalNumberOfProductsPrefixCacheKey);

    public static string CategoryTotalNumberOfProductsPrefixCacheKey => "Nop.productcategory.numberofproducts";

    public static CacheKey CategoryTemplatesAllCacheKey => new CacheKey("Nop.categorytemplate.all");

    #endregion
}
