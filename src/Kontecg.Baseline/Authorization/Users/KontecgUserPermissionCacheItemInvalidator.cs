using System.Collections.Generic;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Organizations;
using Kontecg.Runtime.Caching;

namespace Kontecg.Authorization.Users
{
    public class KontecgUserPermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<UserPermissionSetting>>,
        IEventHandler<EntityChangedEventData<UserRole>>,
        IEventHandler<EntityChangedEventData<UserOrganizationUnit>>,
        IEventHandler<EntityDeletedEventData<KontecgUserBase>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        public KontecgUserPermissionCacheItemInvalidator(
            ICacheManager cacheManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    List<UserOrganizationUnit> users = _userOrganizationUnitRepository.GetAllList(userOu =>
                        userOu.OrganizationUnitId == eventData.Entity.OrganizationUnitId
                    );

                    foreach (UserOrganizationUnit userOrganizationUnit in users)
                    {
                        string cacheKey = userOrganizationUnit.UserId + "@" + (eventData.Entity.CompanyId ?? 0);
                        _cacheManager.GetUserPermissionCache().Remove(cacheKey);
                    }
                }
            });
        }

        public void HandleEvent(EntityChangedEventData<UserOrganizationUnit> eventData)
        {
            string cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.CompanyId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<UserPermissionSetting> eventData)
        {
            string cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.CompanyId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            string cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.CompanyId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<KontecgUserBase> eventData)
        {
            string cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.CompanyId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }
    }
}
