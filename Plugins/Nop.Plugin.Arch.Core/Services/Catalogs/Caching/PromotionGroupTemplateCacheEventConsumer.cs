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
/// Represents a PromotionGroup template cache event consumer
/// </summary>
public partial class PromotionGroupTemplateCacheEventConsumer : CacheEventConsumer<PromotionGroupTemplate>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    protected override async Task ClearCacheAsync(PromotionGroupTemplate entity)
    {
        await RemoveAsync(NopCatalogDefaultsAdditional.PromotionGroupTemplatesAllCacheKey);        
    }
}
