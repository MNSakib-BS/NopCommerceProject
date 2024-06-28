using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Promotions;
using Nop.Services.Caching;
using Nop.Services.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs.Caching;
/// <summary>
/// Represents a product PromotionGroup cache event consumer
/// </summary>
/// <summary>
/// Represents a product PromotionGroup cache event consumer
/// </summary>
public partial class ProductPromotionGroupCacheEventConsumer : CacheEventConsumer<ProductPromotionGroup>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    protected override async Task ClearCacheAsync(ProductPromotionGroup entity, EntityEventType entityEventType)
    { 
        await RemoveByPrefixAsync(NopCatalogDefaultsAdditional.ProductPromotionGroupsByProductPrefixCacheKey, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaultsAdditional.ProductPricePrefixCacheKey, entity.ProductId);        
    }
}
