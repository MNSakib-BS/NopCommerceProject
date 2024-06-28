using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.MegaMenu.Infrastructure.Cache;
public class MegaMenuModelCacheEventConsumer :
       IConsumer<EntityInsertedEvent<Manufacturer>>,
       IConsumer<EntityUpdatedEvent<Manufacturer>>,
       IConsumer<EntityDeletedEvent<Manufacturer>>,
       IConsumer<EntityInsertedEvent<Category>>,
       IConsumer<EntityUpdatedEvent<Category>>,
       IConsumer<EntityDeletedEvent<Category>>,
       IConsumer<EntityInsertedEvent<Topic>>,
       IConsumer<EntityUpdatedEvent<Topic>>,
       IConsumer<EntityDeletedEvent<Topic>>
{
    /// <summary>
    /// Key for caching
    /// </summary>
    /// <remarks>
    /// {0} : language id
    /// {1} : customer role ids
    /// {2} : store id
    /// </remarks>
    public static CacheKey MEGAMENU_TOPICS_MODEL_KEY = new CacheKey("Nopstation.megamenu.topic-{0}-{1}-{2}", MEGAMENU_TOPICS_PATTERN_KEY);
    public const string MEGAMENU_TOPICS_PATTERN_KEY = "Nopstation.megamenu.topic";

    /// <summary>
    /// Key for caching
    /// </summary>
    /// <remarks>
    /// {0} : language id
    /// {1} : customer role ids
    /// {2} : store id
    /// </remarks>
    public static CacheKey MEGAMENU_CATEGORIES_MODEL_KEY = new CacheKey("Nopstation.megamenu.categories-{0}-{1}-{2}", MEGAMENU_CATEGORIES_PATTERN_KEY);
    public const string MEGAMENU_CATEGORIES_PATTERN_KEY = "Nopstation.megamenu.categories";

    /// <summary>
    /// Key for caching
    /// </summary>
    /// <remarks>
    /// {0} : language id
    /// {1} : customer role ids
    /// {2} : store id
    /// </remarks>
    public static CacheKey MEGAMENU_MANUFACTURERS_MODEL_KEY = new CacheKey("Nopstation.megamenu.manufacturers-{0}-{1}-{2}", MEGAMENU_MANUFACTURERS_PATTERN_KEY);
    public const string MEGAMENU_MANUFACTURERS_PATTERN_KEY = "Nopstation.megamenu.manufacturers";

    /// <summary>
    /// Key for caching
    /// </summary>
    /// <remarks>
    /// {0} : language id
    /// {1} : customer role ids
    /// {2} : store id
    /// </remarks>
    public static CacheKey MEGAMENU_MODEL_KEY = new CacheKey("Nopstation.megamenu.all-{0}-{1}", MEGAMENU_PATTERN_KEY);
    public const string MEGAMENU_PATTERN_KEY = "Nopstation.megamenu.";

    private readonly IStaticCacheManager _cacheManager;

    public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_MANUFACTURERS_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_MANUFACTURERS_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_MANUFACTURERS_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_CATEGORIES_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_CATEGORIES_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_CATEGORIES_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Topic> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_TOPICS_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Topic> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_TOPICS_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Topic> eventMessage)
    {
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_TOPICS_PATTERN_KEY);
        await _cacheManager.RemoveByPrefixAsync(MEGAMENU_PATTERN_KEY);
    }
}
