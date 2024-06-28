using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Arch.Core.Services.Stores;
public interface IStoreContextAdditional
{
    int StoreTypeId { get; }

    /// <summary>
    /// Overrides the cached store with the passed-in store.
    /// </summary>
    Task ReplaceAsync(Store store);

    /// <summary>
    /// Gets the global store asynchronously.
    /// </summary>
    Task<Store> GetGlobalStoreAsync();
}

