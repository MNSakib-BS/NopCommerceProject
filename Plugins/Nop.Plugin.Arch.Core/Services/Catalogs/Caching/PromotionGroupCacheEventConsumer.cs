using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Promotions;
using Nop.Plugin.Arch.Core.Services.Discounts;
using Nop.Services.Caching;
using Nop.Services.Discounts;

namespace Nop.Plugin.Arch.Core.Services.Catalogs.Caching;
/// <summary>
/// Represents a PromotionGroup cache event consumer
/// </summary>
public partial class PromotionGroupCacheEventConsumer : CacheEventConsumer<PromotionGroup>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    protected override async Task ClearCacheAsync(PromotionGroup entity)
    {
        await RemoveByPrefixAsync(NopDiscountDefaultsAdditional.DiscountPromotionGroupIdsPrefixCacheKey);
    }
}
