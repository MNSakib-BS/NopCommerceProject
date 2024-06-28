using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;

namespace Nop.Plugin.Arch.Core.Services.Topics;
/// <summary>
/// Represents default values related to topic services
/// </summary>
public static partial class NopTopicDefaults
{
    #region Caching defaults

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : store ID
    /// {1} : show hidden?
    /// {2} : include in top menu?
    /// {3} : include terms and conditions?
    /// </remarks>
    public static CacheKey TopicsAllCacheKey => new CacheKey("Nop.topics.all-{0}-{1}-{2}-{3}", TopicsAllPrefixCacheKey);

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : store ID
    /// {1} : show hidden?
    /// {2} : include in top menu?
    /// {3} : include terms and conditions?
    /// {4} : customer role IDs hash
    /// </remarks>
    public static CacheKey TopicsAllWithACLCacheKey => new CacheKey("Nop.topics.all.acl-{0}-{1}-{2}-{3}-{4}", TopicsAllPrefixCacheKey);

    /// <summary>
    /// Gets a pattern to clear cache
    /// </summary>
    public static string TopicsAllPrefixCacheKey => "Nop.topics.all";

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : topic system name
    /// {1} : store id
    /// {2} : customer roles Ids hash
    /// </remarks>
    public static CacheKey TopicBySystemNameCacheKey => new CacheKey("Nop.topics.systemName-{0}-{1}-{2}", TopicBySystemNamePrefixCacheKey);

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : topic system name
    /// </remarks>
    public static string TopicBySystemNamePrefixCacheKey => "Nop.topics.systemName-{0}";

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    public static CacheKey TopicTemplatesAllCacheKey => new CacheKey("Nop.topictemplates.all");

    #endregion
}
