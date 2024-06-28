using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Payments;
using Nop.Plugin.Arch.Core.Domains.Stores;
using Nop.Plugin.Arch.Core.Services.Caching.Extensions;
using Nop.Plugin.Arch.Core.Services.Customers;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.Stores;
public class StoreMappingAdditionalService : IStoreMappingAdditionalService
{
    #region Fields

    private readonly IRepository<CustomerWalletStoreMapping> _customerWalletStoreMappingRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly CatalogSettings _catalogSettings;
    private readonly IRepository<StoreMapping> _storeMappingRepository;
    private readonly IWorkContext _workContext;
    private readonly ICustomerAdditionalService _customerAdditionalService;
    private readonly ICacheKeyService _cacheKeyService;

    #endregion

    #region Ctor

    public StoreMappingAdditionalService(IRepository<CustomerWalletStoreMapping> customerWalletStoreMappingRepository,
         IEventPublisher eventPublisher,
         CatalogSettings catalogSettings,
         IRepository<StoreMapping> storeMappingRepository,
         IWorkContext workContext,
         ICustomerAdditionalService customerAdditionalService,
         ICacheKeyService cacheKeyService)
    {
        _customerWalletStoreMappingRepository = customerWalletStoreMappingRepository;
        _eventPublisher = eventPublisher;
        _catalogSettings = catalogSettings;
        _storeMappingRepository = storeMappingRepository;
        _workContext = workContext;
        _customerAdditionalService = customerAdditionalService;
        _cacheKeyService = cacheKeyService;
    }

    #endregion

    #region Methods


    /// <summary>
    /// Deletes a customer wallet store mapping record
    /// </summary>
    /// <param name="storeMapping">Store mapping record</param>
    public virtual async Task DeleteCustomerWalletStoreMappingAsync(CustomerWalletStoreMapping storeMapping)
    {
        if (storeMapping == null)
            throw new ArgumentNullException(nameof(storeMapping));

        await _customerWalletStoreMappingRepository.DeleteAsync(storeMapping);

        //event notification
        await _eventPublisher.EntityDeletedAsync(storeMapping);
    }

    /// <summary>
    /// Filter a query by store
    /// </summary>
    /// <param name="query">Unfiltered query</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="onlyShowSelectedStore">Select whether to ignore shipments with no store limitations on filter</param>
    /// <returns>Filtered data</returns>
    public virtual async Task<IQueryable<T>> FilterStoresAsync<T>(IQueryable<T> query, int storeId = 0, bool onlyShowSelectedStore = false) where T : BaseEntity, IStoreMappingSupported
    {
        if (_catalogSettings.IgnoreStoreLimitations)
        {
            return query;
        }

        if (storeId > 0)
        {

            if (onlyShowSelectedStore)
            {
                //Store mapping
                query = from entity in query
                        join sm in _storeMappingRepository.Table
                            on new { c1 = entity.Id, c2 = typeof(T).Name } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into entity_sm
                        from sm in entity_sm.DefaultIfEmpty()
                        where storeId == sm.StoreId
                        select entity;
            }
            else
            {
                //Store mapping
                query = from entity in query
                        join sm in _storeMappingRepository.Table
                            on new { c1 = entity.Id, c2 = typeof(T).Name } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into entity_sm
                        from sm in entity_sm.DefaultIfEmpty()
                        where !entity.LimitedToStores || storeId == sm.StoreId
                        select entity;
            }

            query = query.Distinct();
        }
        else
        {
            // if current customer is limited to stores and all stores is selected i.e. storeId == 0
            // then filter the data on all the stores associated with the customer

            
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var customerAdditional=await _customerAdditionalService.GetCustomerAddiitonalByCustomerIdAsync(currentCustomer.Id);

            if (customerAdditional.LimitedToStores)
            {
                var customerLimitedStores = (
                    from d in _storeMappingRepository.Table
                    where d.EntityName == "Customer" && d.EntityId == currentCustomer.Id
                    select d.StoreId
                ).ToArray();

                //Store mapping
                query = from entity in query
                        join sm in _storeMappingRepository.Table
                            on new { c1 = entity.Id, c2 = typeof(T).Name } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into entity_sm
                        from sm in entity_sm.DefaultIfEmpty()
                        where !entity.LimitedToStores || customerLimitedStores.Contains(sm.StoreId)
                        select entity;

                query = query.Distinct();
            }
        }

        return query;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="storeId"></param>
    /// <param name="onlyShowSelectedStore"></param>
    /// <returns></returns>
    public virtual async Task<IQueryable<T>> FilterStoresForCustomerWalletAsync<T>(IQueryable<T> query, int storeId = 0, bool onlyShowSelectedStore = false) where T : BaseEntity, IStoreMappingSupported
    {
        var storeMappings = await _customerWalletStoreMappingRepository.Table
                                     .Where(sm => sm.StoreId == storeId)
                                     .ToListAsync();

        var filteredQuery = from entity in query
                            join sm in storeMappings
                                on entity.Id equals sm.CustomerWalletId into entity_sm
                            from sm in entity_sm.DefaultIfEmpty()
                            select entity;

        filteredQuery = filteredQuery.Distinct();

        return filteredQuery;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IList<CustomerWalletStoreMapping>> GetCustomerWalletStoreMappingsAsync(CustomerWallet entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var customerWalletId = entity.Id;


        var key = _cacheKeyService.PrepareKeyForDefaultCache(NopStoreDefaultsAdditional.StoreMappingsByCustomerWalletIdCacheKey, customerWalletId);

        var query = from sm in _customerWalletStoreMappingRepository.Table
                    where sm.CustomerWalletId == customerWalletId
                    select sm;

        var storeMappings =await query.ToCachedListAsync(key);

        return  storeMappings;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<IList<CustomerWalletStoreMapping>> GetCustomerWalletStoreMappingsAsync(CustomerWalletTransaction entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var customerWalletId = entity.CustomerWalletId;


        var key = _cacheKeyService.PrepareKeyForDefaultCache(NopStoreDefaultsAdditional.StoreMappingsByCustomerWalletIdCacheKey, customerWalletId);

        var query = from sm in _customerWalletStoreMappingRepository.Table
                    where sm.CustomerWalletId == customerWalletId
                    select sm;

        var storeMappings =await query.ToCachedListAsync(key);

        return storeMappings;
    }


    /// <summary>
    /// Inserts a customer wallet store mapping record
    /// </summary>
    /// <param name="storeMapping">Store mapping</param>
    protected virtual async Task InsertCustomerWalletStoreMappingAsync(CustomerWalletStoreMapping storeMapping)
    {
        if (storeMapping == null)
            throw new ArgumentNullException(nameof(storeMapping));

        await _customerWalletStoreMappingRepository.InsertAsync(storeMapping);

        //event notification
        _eventPublisher.EntityInserted(storeMapping);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="storeId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task InsertCustomerWalletStoreMappingAsync<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (storeId == 0)
            throw new ArgumentOutOfRangeException(nameof(storeId));

        var customerWalletId = entity.Id;

        var customerWalletStoreMapping = new CustomerWalletStoreMapping
        {
            CustomerWalletId = customerWalletId,
            StoreId = storeId
        };

        await InsertCustomerWalletStoreMappingAsync(customerWalletStoreMapping);
    }

    #endregion

}
