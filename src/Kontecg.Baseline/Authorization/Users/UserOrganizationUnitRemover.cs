using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Removes the user from all organization units when a user is deleted.
    /// </summary>
    public class UserOrganizationUnitRemover :
        IEventHandler<EntityDeletedEventData<KontecgUserBase>>,
        ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        public UserOrganizationUnitRemover(
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual void HandleEvent(EntityDeletedEventData<KontecgUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    _userOrganizationUnitRepository.Delete(
                        uou => uou.UserId == eventData.Entity.Id
                    );
                }
            });
        }
    }
}
