using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2016.Excel;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Arch.Core.Services.Caching.Extensions;
using Nop.Services.Common;

namespace Nop.Plugin.Arch.Core.Services.Common;
public class GenericAttributeAdditionalService : IGenericAttributeAdditionalService
{
    #region Fields

    private readonly IEventPublisher _eventPublisher;
    private readonly IRepository<GenericAttribute> _genericAttributeRepository;
    private readonly IGenericAttributeService _genericAttributeService;

    #endregion

    #region Ctor

    public GenericAttributeAdditionalService(IEventPublisher eventPublisher,
            IRepository<GenericAttribute> genericAttributeRepository,
            IGenericAttributeService genericAttributeService)
    {
        _eventPublisher = eventPublisher;
        _genericAttributeRepository = genericAttributeRepository;
        _genericAttributeService = genericAttributeService;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    public virtual void DeleteAttributeByEntityId(string key, int entityId)
    {
        var attribute = _genericAttributeRepository.Table.FirstOrDefault(i => i.Key == key && i.EntityId == entityId);

        if (attribute != null)
           _genericAttributeService.DeleteAttributeAsync(attribute);
    }


    /// <summary>
    /// Deletes attributes
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="key">Key</param>
    /// <param name="storeId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
    public virtual async Task DeleteAttributesAsync(BaseEntity entity, string key, int storeId = 0)
    {
        if (entity == null)
            return;

        var keyGroup = entity.GetType().Name;

        var props = await _genericAttributeService.GetAttributesForEntityAsync(entity.Id, keyGroup);

        //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
        if (props == null)
            return;

        props = props.Where(x => x.StoreId == storeId && x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).ToList();
        if (!props.Any())
            return;

        await _genericAttributeRepository.DeleteAsync(props);

        //event notification
        foreach (var attribute in props)
        {
           await  _eventPublisher.EntityDeletedAsync(attribute);
        }
    }

    public virtual void DeleteAttributesByKey(string key)
    {
        var attributes = _genericAttributeRepository.Table.Where(i => i.Key == key);

        foreach (var item in attributes)
        {
           _genericAttributeService.DeleteAttributeAsync(item);
        }
    }

    public async Task<IList<GenericAttribute>> GetAttributesByKeyAsync(string key, string keyGroup, int? storeId = null)
    {
        //we cannot inject ICacheKeyService into constructor because it'll cause circular references.
        //that's why we resolve it here this way
        string storeKey = storeId.HasValue ? storeId.Value.ToString() : "0";
        var cacheKey = EngineContext.Current.Resolve<ICacheKeyService>()
            .PrepareKeyForDefaultCache(NopCommonDefaults.GenericAttributeCacheKey, $"{key}-{storeKey}", keyGroup);

        var query = from ga in _genericAttributeRepository.Table
                    where ga.Key == key &&
                          ga.KeyGroup == keyGroup
                    select ga;

        if (storeId != null)
        {
            query = query.Where(i => i.StoreId == storeId);
        }

        return await query.ToCachedListAsync(cacheKey);       
    }


    /// <summary>
    /// Get attributes
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="keyGroup">Key group</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Generic attributes</returns>
    public virtual  IList<GenericAttribute> GetAttributesByKey(string key, int storeId = 0, DateTime? lastUpdatedFromUtc = null, DateTime? lastUpdatedToUtc = null)
    {
        var query = from ga in _genericAttributeRepository.Table
                    where ga.Key == key && ga.StoreId == storeId
                    select ga;

        if (lastUpdatedFromUtc.HasValue)
            query = query.Where(i => i.CreatedOrUpdatedDateUTC >= lastUpdatedFromUtc);

        if (lastUpdatedToUtc.HasValue)
            query = query.Where(i => i.CreatedOrUpdatedDateUTC <= lastUpdatedToUtc);

        return query.ToList();
    }

    /// <summary>
    /// Get attributes
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="keyGroup">Key group</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Generic attributes</returns>
    public virtual async Task<IList<GenericAttribute>> GetAttributesByKeyValueAsync(string key, string value, string keyGroup, int? storeId = null)
    {
        //we cannot inject ICacheKeyService into constructor because it'll cause circular references.
        //that's why we resolve it here this way
        var cacheKey = EngineContext.Current.Resolve<ICacheKeyService>()
            .PrepareKeyForDefaultCache(NopCommonDefaults.GenericAttributeCacheKey, $"{key}-{value}", keyGroup);

        var query = from ga in _genericAttributeRepository.Table
                    where ga.Key == key &&
                          ga.Value == value &&
                          ga.KeyGroup == keyGroup
                    select ga;

        if (storeId != null)
        {
            query = query.Where(i => i.StoreId == storeId);
        }

        return await query.ToCachedListAsync(cacheKey);
        
    }

    #endregion


}
