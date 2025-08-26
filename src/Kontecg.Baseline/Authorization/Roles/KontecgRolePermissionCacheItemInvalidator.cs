using Kontecg.Dependency;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Organizations;
using Kontecg.Runtime.Caching;

namespace Kontecg.Authorization.Roles
{
    public class KontecgRolePermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<RolePermissionSetting>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        IEventHandler<EntityDeletedEventData<KontecgRoleBase>>,
        ITransientDependency
    {
        private readonly ICacheManager _cacheManager;

        public KontecgRolePermissionCacheItemInvalidator(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            string cacheKey = eventData.Entity.RoleId + "@" + (eventData.Entity.CompanyId ?? 0);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<RolePermissionSetting> eventData)
        {
            string cacheKey = eventData.Entity.RoleId + "@" + (eventData.Entity.CompanyId ?? 0);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<KontecgRoleBase> eventData)
        {
            string cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.CompanyId ?? 0);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }
    }
}
