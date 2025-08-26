using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Synchronizes a user's information to user account.
    /// </summary>
    public class UserAccountSynchronizer :
        IEventHandler<EntityCreatedEventData<KontecgUserBase>>,
        IEventHandler<EntityDeletedEventData<KontecgUserBase>>,
        IEventHandler<EntityUpdatedEventData<KontecgUserBase>>,
        IEventHandler<EntityDeletedEventData<KontecgCompanyBase>>,
        ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserAccount, long> _userAccountRepository;

        /// <summary>
        ///     Constructor
        /// </summary>
        public UserAccountSynchronizer(
            IRepository<UserAccount, long> userAccountRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userAccountRepository = userAccountRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        ///     Handles creation event of user
        /// </summary>
        public virtual void HandleEvent(EntityCreatedEventData<KontecgUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    UserAccount userAccount = _userAccountRepository.FirstOrDefault(
                        ua => ua.CompanyId == eventData.Entity.CompanyId && ua.UserId == eventData.Entity.Id
                    );

                    if (userAccount == null)
                    {
                        _userAccountRepository.Insert(new UserAccount
                        {
                            CompanyId = eventData.Entity.CompanyId,
                            UserName = eventData.Entity.UserName,
                            UserId = eventData.Entity.Id,
                            EmailAddress = eventData.Entity.EmailAddress
                        });
                    }
                    else
                    {
                        userAccount.UserName = eventData.Entity.UserName;
                        userAccount.EmailAddress = eventData.Entity.EmailAddress;
                        _userAccountRepository.Update(userAccount);
                    }
                }
            });
        }

        /// <summary>
        ///     Handles deletion event of company
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void HandleEvent(EntityDeletedEventData<KontecgCompanyBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    _userAccountRepository.Delete(ua => ua.CompanyId == eventData.Entity.Id);
                }
            });
        }

        /// <summary>
        ///     Handles deletion event of user
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void HandleEvent(EntityDeletedEventData<KontecgUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    UserAccount userAccount = _userAccountRepository.FirstOrDefault(
                        ua => ua.CompanyId == eventData.Entity.CompanyId && ua.UserId == eventData.Entity.Id
                    );

                    if (userAccount != null)
                    {
                        _userAccountRepository.Delete(userAccount);
                    }
                }
            });
        }

        /// <summary>
        ///     Handles update event of user
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void HandleEvent(EntityUpdatedEventData<KontecgUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    UserAccount userAccount = _userAccountRepository.FirstOrDefault(ua =>
                        ua.CompanyId == eventData.Entity.CompanyId && ua.UserId == eventData.Entity.Id
                    );

                    if (userAccount != null)
                    {
                        userAccount.UserName = eventData.Entity.UserName;
                        userAccount.EmailAddress = eventData.Entity.EmailAddress;
                        _userAccountRepository.Update(userAccount);
                    }
                }
            });
        }
    }
}
