using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Arch.Core.Domains.Affiliates;
using Nop.Plugin.Arch.Core.Domains.Stores;

namespace Nop.Plugin.Arch.Core.Services.Stores;
public interface IStoreAdditionalService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeId"></param>
    /// <returns></returns>
    Task<StoreAdditional> GetStoreAddiitonalByStoreIdAsync(int storeId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<StoreAdditional> GetStoreAddiitonalByIdAsync(int id);
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeAdditional"></param>
    /// <returns></returns>
    Task DeleteStoreAdditionalAsync(StoreAdditional storeAdditional);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeAdditional"></param>
    /// <returns></returns>
    
    Task InsertStoreAdditionalAsync(StoreAdditional storeAdditional);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeAdditional"></param>
    /// <returns></returns>
    
    Task UpdateStoreAdditionalAsync(StoreAdditional storeAdditional);

    Task<IList<StoreAdditional>> GetAllStoresAdditionalAsync();

    /// <summary>
    /// Gets all stores
    /// </summary>
    /// <returns>Stores</returns>
    Task<IList<Store>> GetAllStoresAsync(int? customerId = null);
    /// <summary>
    /// Gets all stores
    /// </summary>
    /// <returns>Stores</returns>
    Task<IList<Store>> GetAllStoresWithoutRestrictionsAsync(int? customerId = null);
}
