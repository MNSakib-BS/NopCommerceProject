using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Core;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.Stores;
public partial class WebStoreContextAdditional : IStoreContextAdditional
{
    #region Fields

    private readonly IRepository<StoreTypeMapping> _storeTypeMappingRepository;
    private readonly IStoreContext _storeContext;
    private readonly IStoreService _storeService;
    private readonly IStoreAdditionalService _storeAdditionalService;
    private Store? _cachedStore;
    private Store? _cachedGlobalStore;
    private int? _cachedActiveStoreScopeConfiguration;
    private int _storeTypeId;

    #endregion

    #region Ctor

    public WebStoreContextAdditional(IRepository<StoreTypeMapping> storeTypeMappingRepository,
        IStoreContext storeContext,
        IStoreService storeService,
        IStoreAdditionalService storeAdditionalService)
    {
        _storeTypeMappingRepository = storeTypeMappingRepository;
        _storeContext = storeContext;
        _storeService = storeService;
        _storeAdditionalService = storeAdditionalService;
    }

    #endregion

    #region Methods

    public int StoreTypeId
    {
        get
        {
            var currentStore = _storeContext.GetCurrentStore();
            var storeTypeMapping = _storeTypeMappingRepository.Table.FirstOrDefault(p => p.StoreId == currentStore.Id);
            if (_storeTypeId == 0 && storeTypeMapping != null)
            {
                _storeTypeId = storeTypeMapping.StoreTypeId;
            }
            return _storeTypeId;
        }
    }

    /// <summary>
    /// Overrides the cached store with the passed in store asynchronously.
    /// </summary>
    public async Task ReplaceAsync(Store store)
    {
        await Task.Run(() => _cachedStore = store);
    }

    /// <summary>
    /// Gets the global store asynchronously.
    /// </summary>
    public async Task<Store> GetGlobalStoreAsync()
    {
        if (_cachedGlobalStore != null)
            return _cachedGlobalStore;

        var allStores = _storeService.GetAllStores();
        var allStoresAdditional = await _storeAdditionalService.GetAllStoresAdditionalAsync();

        var globalStore = (from store in allStores
                   join sa in allStoresAdditional on store.Id equals sa.StoreId
                   where sa.IsGlobalStore==true
                   select store).FirstOrDefault();       

        if (globalStore == null)
        {
            // Load the first found store
            globalStore = allStores.FirstOrDefault();
        }

        _cachedGlobalStore = globalStore ?? throw new Exception("No global store could be loaded");

        return _cachedGlobalStore;
    }

    #endregion
}

