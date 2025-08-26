using Kontecg.Dependency;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Runtime.Caching;

namespace Kontecg.Localization
{
    /// <summary>
    ///     Clears related localization cache when a <see cref="ApplicationLanguageText" /> changes.
    /// </summary>
    public class MultiCompanyLocalizationDictionaryCacheCleaner :
        ITransientDependency,
        IEventHandler<EntityChangedEventData<ApplicationLanguageText>>
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiCompanyLocalizationDictionaryCacheCleaner" /> class.
        /// </summary>
        public MultiCompanyLocalizationDictionaryCacheCleaner(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<ApplicationLanguageText> eventData)
        {
            _cacheManager
                .GetMultiCompanyLocalizationDictionaryCache()
                .Remove(MultiCompanyLocalizationDictionaryCacheHelper.CalculateCacheKey(
                    eventData.Entity.CompanyId,
                    eventData.Entity.Source,
                    eventData.Entity.LanguageName)
                );
        }
    }
}
