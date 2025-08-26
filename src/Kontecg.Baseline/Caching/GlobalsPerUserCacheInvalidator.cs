using Kontecg.Authorization.Users;
using Kontecg.CachedUniqueKeys;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Localization;
using Kontecg.Organizations;

namespace Kontecg.Caching
{
    public class GlobalsPerUserCacheInvalidator :
        IEventHandler<EntityChangedEventData<UserPermissionSetting>>,
        IEventHandler<EntityChangedEventData<UserRole>>,
        IEventHandler<EntityChangedEventData<UserOrganizationUnit>>,
        IEventHandler<EntityDeletedEventData<KontecgUserBase>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        IEventHandler<EntityChangedEventData<LanguageInfo>>,
        IEventHandler<EntityChangedEventData<SettingInfo>>,
        ITransientDependency
    {
        private const string CacheName = "GlobalsPerUser";

        private readonly ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;

        public GlobalsPerUserCacheInvalidator(ICachedUniqueKeyPerUser cachedUniqueKeyPerUser)
        {
            _cachedUniqueKeyPerUser = cachedUniqueKeyPerUser;
        }

        public void HandleEvent(EntityChangedEventData<UserPermissionSetting> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.CompanyId, eventData.Entity.UserId);
        }

        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.CompanyId, eventData.Entity.UserId);
        }

        public void HandleEvent(EntityChangedEventData<UserOrganizationUnit> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.CompanyId, eventData.Entity.UserId);
        }

        public void HandleEvent(EntityDeletedEventData<KontecgUserBase> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.CompanyId, eventData.Entity.Id);
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            _cachedUniqueKeyPerUser.ClearCache(CacheName);
        }

        public void HandleEvent(EntityChangedEventData<LanguageInfo> eventData)
        {
            _cachedUniqueKeyPerUser.ClearCache(CacheName);
        }

        public void HandleEvent(EntityChangedEventData<SettingInfo> eventData)
        {
            _cachedUniqueKeyPerUser.ClearCache(CacheName);
        }
    }
}
