using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace Nop.Plugin.NopStation.Theme.Arch.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Setting>>,
        IConsumer<EntityUpdatedEvent<Setting>>,
        IConsumer<EntityDeletedEvent<Setting>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : connection type (http/https)
        /// </remarks>
        public static CacheKey PICTURE_URL_MODEL_KEY = new CacheKey("Nop.plugin.nopstation.theme.Arch.pictureurl-{0}-{1}", PICTURE_URL_PATTERN_KEY);
        public const string PICTURE_URL_PATTERN_KEY = "Nop.plugin.nopstation.theme.Arch.pictureurl";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store id
        /// {1} : language id
        /// </remarks>
        public static CacheKey FOOTER_DESCRIPTION_MODEL_KEY = new CacheKey("Nop.plugin.nopstation.theme.Arch.footer.description-{0}-{1}", FOOTER_DESCRIPTION_PATTERN_KEY);
        public const string FOOTER_DESCRIPTION_PATTERN_KEY = "Nop.plugin.nopstation.theme.Arch.footer.description";

        private readonly IStaticCacheManager _staticCacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public async Task HandleEventAsync(EntityInsertedEvent<Setting> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(PICTURE_URL_PATTERN_KEY);
            await _staticCacheManager.RemoveByPrefixAsync(FOOTER_DESCRIPTION_PATTERN_KEY);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(PICTURE_URL_PATTERN_KEY);
            await _staticCacheManager.RemoveByPrefixAsync(FOOTER_DESCRIPTION_PATTERN_KEY);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Setting> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(PICTURE_URL_PATTERN_KEY);
            await _staticCacheManager.RemoveByPrefixAsync(FOOTER_DESCRIPTION_PATTERN_KEY);
        }
    }
}
