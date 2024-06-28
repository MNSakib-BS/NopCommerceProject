using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;
using Nop.Plugin.Arch.Core.Domains.Stores;
using Nop.Plugin.Arch.Core.Domains.Payments;

namespace Nop.Plugin.Arch.Core.Services.Stores;
public interface IStoreMappingAdditionalService
{
    /// <summary>
    /// Filter a query by store
    /// </summary>
    /// <param name="query">Unfiltered query</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Filtered data</returns>
    Task<IQueryable<T>> FilterStoresAsync<T>(IQueryable<T> query, int storeId = 0, bool onlyShowSelectedStore = false) where T : BaseEntity, IStoreMappingSupported;
    Task<IQueryable<T>> FilterStoresForCustomerWalletAsync<T>(IQueryable<T> query, int storeId = 0, bool onlyShowSelectedStore = false) where T : BaseEntity, IStoreMappingSupported;

    Task DeleteCustomerWalletStoreMappingAsync(CustomerWalletStoreMapping storeMapping);

    Task InsertCustomerWalletStoreMappingAsync<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported;

    Task<IList<CustomerWalletStoreMapping>> GetCustomerWalletStoreMappingsAsync(CustomerWallet entity);

    Task<IList<CustomerWalletStoreMapping>> GetCustomerWalletStoreMappingsAsync(CustomerWalletTransaction entity);
}
