using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Arch.Core.Services.Common;
public partial interface IGenericAttributeAdditionalService
{
    /// <summary>
    /// Deletes attributes
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="key">Key</param>
    /// <param name="storeId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
    Task DeleteAttributesAsync(BaseEntity entity, string key, int storeId = 0);

    /// <summary>
    /// Get attributes
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="keyGroup">Key group</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Generic attributes</returns>
    Task<IList<GenericAttribute>> GetAttributesByKeyValueAsync(string key, string value, string keyGroup, int? storeId = null);

    /// <summary>
    /// Get attributes
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="keyGroup">Key group</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Generic attributes</returns>
    Task<IList<GenericAttribute>> GetAttributesByKeyAsync(string key, string keyGroup, int? storeId = null);

    IList<GenericAttribute> GetAttributesByKey(string key, int storeId = 0, DateTime? lastUpdatedFromUtc = null, DateTime? lastUpdatedToUtc = null);
    void DeleteAttributeByEntityId(string key, int entityId);
    void DeleteAttributesByKey(string key);
}
