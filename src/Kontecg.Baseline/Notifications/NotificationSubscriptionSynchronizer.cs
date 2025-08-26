using System;
using Kontecg.Authorization.Users;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Notifications
{
    public class NotificationSubscriptionSynchronizer : IEventHandler<EntityDeletedEventData<KontecgUserBase>>,
        ITransientDependency
    {
        private readonly IRepository<NotificationSubscriptionInfo, Guid> _notificationSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificationSubscriptionSynchronizer(
            IRepository<NotificationSubscriptionInfo, Guid> notificationSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager
        )
        {
            _notificationSubscriptionRepository = notificationSubscriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual void HandleEvent(EntityDeletedEventData<KontecgUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    _notificationSubscriptionRepository.Delete(x => x.UserId == eventData.Entity.Id);
                }
            });
        }
    }
}
