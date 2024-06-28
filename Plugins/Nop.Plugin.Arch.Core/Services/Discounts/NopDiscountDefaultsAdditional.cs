using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Services.Discounts;
public static class NopDiscountDefaultsAdditional
{
    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string DiscountPromotionGroupIdsPrefixCacheKey => "Nop.discounts.promotiongroupids";
}
