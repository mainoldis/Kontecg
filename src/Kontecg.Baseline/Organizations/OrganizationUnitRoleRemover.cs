using Kontecg.Authorization.Roles;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Organizations
{
    /// <summary>
    ///     Removes the role from all organization units when a role is deleted.
    /// </summary>
    public class OrganizationUnitRoleRemover :
        IEventHandler<EntityDeletedEventData<KontecgRoleBase>>,
        ITransientDependency
    {
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrganizationUnitRoleRemover(
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual void HandleEvent(EntityDeletedEventData<KontecgRoleBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    _organizationUnitRoleRepository.Delete(
                        uou => uou.RoleId == eventData.Entity.Id
                    );
                }
            });
        }
    }
}
