using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Domains.Stores;
using Nop.Plugin.Arch.Core.Services.Caching.Extensions;

namespace Nop.Plugin.Arch.Core.Services.Stores;
public class StoreAdditionalService : IStoreAdditionalService
{
    #region Fields

    private readonly IRepository<StoreAdditional> _storeAdditionalRepository;
    private readonly IRepository<Store> _storeRepository;


    #endregion

    #region Ctor

    public StoreAdditionalService(IRepository<StoreAdditional> storeAdditionalRepository,
        IRepository<Store> storeRepository)
    {
        _storeAdditionalRepository = storeAdditionalRepository;
        _storeRepository = storeRepository;
    }

    #endregion

    #region Utilities

    public virtual int[] GetStoresIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;

        var storeMappingRepo = EngineContext.Current.Resolve<IRepository<StoreMapping>>();

        var query = from sm in storeMappingRepo.Table
                    where sm.EntityId == entityId &&
                          sm.EntityName == entityName
                    select sm.StoreId;

        return query.ToArray();
    }

    #endregion

    #region Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeAdditional"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task DeleteStoreAdditionalAsync(StoreAdditional storeAdditional)
    {
        await _storeAdditionalRepository.DeleteAsync(storeAdditional);
    }

    public virtual async Task<IList<StoreAdditional>> GetAllStoresAdditionalAsync()
    {
        return await _storeAdditionalRepository.GetAllAsync(query =>
        {
            return from s in query select s;
        });
    }

    public virtual async Task<IList<Store>> GetAllStoresAsync(int? customerId = null)
    {
        var query = from s in _storeRepository.Table orderby s.DisplayOrder, s.Id select s;

        //we can not use ICacheKeyService because it'll cause circular references.
        //that's why we use the default cache time
        var result = await query.ToCachedListAsync(NopStoreDefaults.StoresAllCacheKey);

        var accessableStores = new List<Store>();

        if (!customerId.HasValue)
        {
            foreach (var item in result)
            {
                accessableStores.Add(item);
            }
        }
        else
        {
            var customersStores = GetStoresIdsWithAccess(new CustomerAdditional { Id = customerId.Value });
            var restrictStores = customersStores.Length > 0;

            foreach (var item in result)
            {
                if (!restrictStores || customersStores.Contains(item.Id))
                {
                    accessableStores.Add(item);
                }
            }
        }

        return accessableStores;
    }

    public virtual async Task<IList<Store>> GetAllStoresWithoutRestrictionsAsync(int? customerId = null)
    {
        var query = from s in _storeRepository.Table orderby s.DisplayOrder, s.Id select s;

        var result =await query.ToCachedListAsync(NopStoreDefaults.StoresAllCacheKey);

        var accessableStores = new List<Store>();

        foreach (var item in result)
        {
            accessableStores.Add(item);
        }

        return accessableStores;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<StoreAdditional> GetStoreAddiitonalByIdAsync(int id)
    {
        return await _storeAdditionalRepository.GetByIdAsync(id, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<StoreAdditional> GetStoreAddiitonalByStoreIdAsync(int storeId)
    {
        return await _storeAdditionalRepository.Table.Where(e => e.StoreId == storeId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeAdditional"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task InsertStoreAdditionalAsync(StoreAdditional storeAdditional)
    {
        await _storeAdditionalRepository.InsertAsync(storeAdditional);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeAdditional"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task UpdateStoreAdditionalAsync(StoreAdditional storeAdditional)
    {
        await _storeAdditionalRepository.UpdateAsync(storeAdditional);
    }

    #endregion
}
