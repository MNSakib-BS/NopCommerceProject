using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;

namespace Nop.Plugin.Arch.Core.Services.Stores;
public static partial class NopStoreDefaults
{
    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity ID
    /// {1} : entity name
    /// </remarks>
    public static CacheKey StoreMappingsByCustomerWalletIdCacheKey => new CacheKey("Nop.storemapping.entityid-name-{0}");

    #region Stores

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    public static CacheKey StoresAllCacheKey => new CacheKey("Nop.stores.all");

    #endregion
}
