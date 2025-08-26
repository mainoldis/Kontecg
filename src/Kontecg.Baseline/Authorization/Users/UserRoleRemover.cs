using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Removes the user from all user roles when a user is deleted.
    /// </summary>
    public class UserRoleRemover :
        IEventHandler<EntityDeletedEventData<KontecgUserBase>>,
        ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        public UserRoleRemover(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserRole, long> userRoleRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userRoleRepository = userRoleRepository;
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<KontecgUserBase> eventData)
        {
            using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
            {
                _userRoleRepository.Delete(
                    ur => ur.UserId == eventData.Entity.Id
                );
            }
        }
    }
}
