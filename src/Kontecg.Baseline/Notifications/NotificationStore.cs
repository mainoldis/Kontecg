using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using Kontecg.Linq.Extensions;
using LinqKit;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Implements <see cref="INotificationStore" /> using repositories.
    /// </summary>
    public class NotificationStore : INotificationStore, ITransientDependency
    {
        private readonly IRepository<CompanyNotificationInfo, Guid> _companyNotificationRepository;

        private readonly IRepository<NotificationInfo, Guid> _notificationRepository;
        private readonly IRepository<NotificationSubscriptionInfo, Guid> _notificationSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserNotificationInfo, Guid> _userNotificationRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationStore" /> class.
        /// </summary>
        public NotificationStore(
            IRepository<NotificationInfo, Guid> notificationRepository,
            IRepository<CompanyNotificationInfo, Guid> companyNotificationRepository,
            IRepository<UserNotificationInfo, Guid> userNotificationRepository,
            IRepository<NotificationSubscriptionInfo, Guid> notificationSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _notificationRepository = notificationRepository;
            _companyNotificationRepository = companyNotificationRepository;
            _userNotificationRepository = userNotificationRepository;
            _notificationSubscriptionRepository = notificationSubscriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public virtual async Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(subscription.CompanyId))
                {
                    await _notificationSubscriptionRepository.InsertAsync(subscription);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void InsertSubscription(NotificationSubscriptionInfo subscription)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(subscription.CompanyId))
                {
                    _notificationSubscriptionRepository.Insert(subscription);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task DeleteSubscriptionAsync(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    await _notificationSubscriptionRepository.DeleteAsync(s =>
                        s.UserId == user.UserId &&
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );

                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void DeleteSubscription(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    _notificationSubscriptionRepository.Delete(s =>
                        s.UserId == user.UserId &&
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );

                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task InsertNotificationAsync(NotificationInfo notification)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    await _notificationRepository.InsertAsync(notification);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void InsertNotification(NotificationInfo notification)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    _notificationRepository.Insert(notification);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    return await _notificationRepository.FirstOrDefaultAsync(notificationId);
                }
            });
        }

        public virtual NotificationInfo GetNotificationOrNull(Guid notificationId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    return _notificationRepository.FirstOrDefault(notificationId);
                }
            });
        }

        public virtual async Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(userNotification.CompanyId))
                {
                    await _userNotificationRepository.InsertAsync(userNotification);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void InsertUserNotification(UserNotificationInfo userNotification)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(userNotification.CompanyId))
                {
                    _userNotificationRepository.Insert(userNotification);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                {
                    return await _notificationSubscriptionRepository.GetAllListAsync(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }
            });
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                {
                    return _notificationSubscriptionRepository.GetAllList(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }
            });
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            int?[] companyIds,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                List<NotificationSubscriptionInfo> subscriptions = new List<NotificationSubscriptionInfo>();

                foreach (int? companyId in companyIds)
                {
                    subscriptions.AddRange(await GetSubscriptionsAsync(companyId, notificationName, entityTypeName,
                        entityId));
                }

                return subscriptions;
            });
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            int?[] companyIds,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                List<NotificationSubscriptionInfo> subscriptions = new List<NotificationSubscriptionInfo>();

                foreach (int? companyId in companyIds)
                {
                    subscriptions.AddRange(GetSubscriptions(companyId, notificationName, entityTypeName, entityId));
                }

                return subscriptions;
            });
        }

        public virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    return await _notificationSubscriptionRepository.GetAllListAsync(s => s.UserId == user.UserId);
                }
            });
        }

        public virtual List<NotificationSubscriptionInfo> GetSubscriptions(UserIdentifier user)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    return _notificationSubscriptionRepository.GetAllList(s => s.UserId == user.UserId);
                }
            });
        }

        public virtual async Task<bool> IsSubscribedAsync(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    return await _notificationSubscriptionRepository.CountAsync(s =>
                        s.UserId == user.UserId &&
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    ) > 0;
                }
            });
        }

        public virtual bool IsSubscribed(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    return _notificationSubscriptionRepository.Count(s =>
                        s.UserId == user.UserId &&
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    ) > 0;
                }
            });
        }

        public virtual async Task UpdateUserNotificationStateAsync(
            int? companyId,
            Guid userNotificationId,
            UserNotificationState state)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    UserNotificationInfo userNotification =
                        await _userNotificationRepository.FirstOrDefaultAsync(userNotificationId);
                    if (userNotification == null)
                    {
                        return;
                    }

                    userNotification.State = state;
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void UpdateUserNotificationState(
            int? companyId,
            Guid userNotificationId,
            UserNotificationState state)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    UserNotificationInfo userNotification =
                        _userNotificationRepository.FirstOrDefault(userNotificationId);
                    if (userNotification == null)
                    {
                        return;
                    }

                    userNotification.State = state;
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    List<UserNotificationInfo> userNotifications = await _userNotificationRepository.GetAllListAsync(
                        un => un.UserId == user.UserId
                    );

                    foreach (UserNotificationInfo userNotification in userNotifications)
                    {
                        userNotification.State = state;
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    List<UserNotificationInfo> userNotifications = _userNotificationRepository.GetAllList(
                        un => un.UserId == user.UserId
                    );

                    foreach (UserNotificationInfo userNotification in userNotifications)
                    {
                        userNotification.State = state;
                    }

                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task DeleteUserNotificationAsync(int? companyId, Guid userNotificationId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    await _userNotificationRepository.DeleteAsync(userNotificationId);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void DeleteUserNotification(int? companyId, Guid userNotificationId)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    _userNotificationRepository.Delete(userNotificationId);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task DeleteAllUserNotificationsAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    Expression<Func<UserNotificationInfo, bool>> predicate =
                        CreateNotificationFilterPredicate(user, state, startDate, endDate);

                    await _userNotificationRepository.DeleteAsync(predicate);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            });
        }

        public virtual void DeleteAllUserNotifications(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    Expression<Func<UserNotificationInfo, bool>> predicate =
                        CreateNotificationFilterPredicate(user, state, startDate, endDate);

                    _userNotificationRepository.Delete(predicate);
                    _unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        public virtual async Task<List<UserNotificationInfoWithNotificationInfo>>
            GetUserNotificationsWithNotificationsAsync(
                UserIdentifier user,
                UserNotificationState? state = null,
                int skipCount = 0,
                int maxResultCount = int.MaxValue,
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            List<UserNotificationInfoWithNotificationInfo> result = _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join companyNotificationInfo in _companyNotificationRepository.GetAll() on userNotificationInfo
                            .CompanyNotificationId equals companyNotificationInfo.Id
                        where userNotificationInfo.UserId == user.UserId
                        orderby companyNotificationInfo.CreationTime descending
                        select new
                        {
                            userNotificationInfo,
                            companyNotificationInfo
                        };

                    if (state.HasValue)
                    {
                        query = query.Where(x => x.userNotificationInfo.State == state.Value);
                    }

                    if (startDate.HasValue)
                    {
                        query = query.Where(x => x.companyNotificationInfo.CreationTime >= startDate);
                    }

                    if (endDate.HasValue)
                    {
                        query = query.Where(x => x.companyNotificationInfo.CreationTime <= endDate);
                    }

                    query = query.PageBy(skipCount, maxResultCount);

                    var list = query.ToList();

                    return list.Select(
                        a => new UserNotificationInfoWithNotificationInfo(
                            a.userNotificationInfo,
                            a.companyNotificationInfo
                        )
                    ).ToList();
                }
            });

            return await Task.FromResult(result);
        }

        public virtual List<UserNotificationInfoWithNotificationInfo> GetUserNotificationsWithNotifications(
            UserIdentifier user,
            UserNotificationState? state = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join companyNotificationInfo in _companyNotificationRepository.GetAll() on userNotificationInfo
                            .CompanyNotificationId equals companyNotificationInfo.Id
                        where userNotificationInfo.UserId == user.UserId
                        orderby companyNotificationInfo.CreationTime descending
                        select new
                        {
                            userNotificationInfo,
                            companyNotificationInfo
                        };

                    if (state.HasValue)
                    {
                        query = query.Where(x => x.userNotificationInfo.State == state.Value);
                    }

                    if (startDate.HasValue)
                    {
                        query = query.Where(x => x.companyNotificationInfo.CreationTime >= startDate);
                    }

                    if (endDate.HasValue)
                    {
                        query = query.Where(x => x.companyNotificationInfo.CreationTime <= endDate);
                    }

                    query = query.PageBy(skipCount, maxResultCount);

                    var list = query.ToList();

                    return list.Select(
                        a => new UserNotificationInfoWithNotificationInfo(
                            a.userNotificationInfo,
                            a.companyNotificationInfo
                        )
                    ).ToList();
                }
            });
        }

        public virtual async Task<int> GetUserNotificationCountAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    Expression<Func<UserNotificationInfo, bool>> predicate =
                        CreateNotificationFilterPredicate(user, state, startDate, endDate);
                    return await _userNotificationRepository.CountAsync(predicate);
                }
            });
        }

        public virtual int GetUserNotificationCount(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    Expression<Func<UserNotificationInfo, bool>> predicate =
                        CreateNotificationFilterPredicate(user, state, startDate, endDate);
                    return _userNotificationRepository.Count(predicate);
                }
            });
        }

        public virtual async Task<UserNotificationInfoWithNotificationInfo>
            GetUserNotificationWithNotificationOrNullAsync(
                int? companyId,
                Guid userNotificationId)
        {
            UserNotificationInfoWithNotificationInfo result = _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join companyNotificationInfo in _companyNotificationRepository.GetAll() on userNotificationInfo
                            .CompanyNotificationId equals companyNotificationInfo.Id
                        where userNotificationInfo.Id == userNotificationId
                        select new
                        {
                            userNotificationInfo,
                            companyNotificationInfo
                        };

                    var item = query.FirstOrDefault();
                    if (item == null)
                    {
                        return null;
                    }

                    return new UserNotificationInfoWithNotificationInfo(
                        item.userNotificationInfo,
                        item.companyNotificationInfo
                    );
                }
            });

            return await Task.FromResult(result);
        }

        public virtual UserNotificationInfoWithNotificationInfo GetUserNotificationWithNotificationOrNull(
            int? companyId,
            Guid userNotificationId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    var query = from userNotificationInfo in _userNotificationRepository.GetAll()
                        join companyNotificationInfo in _companyNotificationRepository.GetAll() on userNotificationInfo
                            .CompanyNotificationId equals companyNotificationInfo.Id
                        where userNotificationInfo.Id == userNotificationId
                        select new
                        {
                            userNotificationInfo,
                            companyNotificationInfo
                        };

                    var item = query.FirstOrDefault();
                    if (item == null)
                    {
                        return null;
                    }

                    return new UserNotificationInfoWithNotificationInfo(
                        item.userNotificationInfo,
                        item.companyNotificationInfo
                    );
                }
            });
        }

        public virtual async Task InsertCompanyNotificationAsync(CompanyNotificationInfo companyNotificationInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyNotificationInfo.CompanyId))
                {
                    await _companyNotificationRepository.InsertAsync(companyNotificationInfo);
                }
            });
        }

        public virtual void InsertCompanyNotification(CompanyNotificationInfo companyNotificationInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyNotificationInfo.CompanyId))
                {
                    _companyNotificationRepository.Insert(companyNotificationInfo);
                }
            });
        }

        public virtual async Task DeleteNotificationAsync(NotificationInfo notification)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _notificationRepository.DeleteAsync(notification));
        }

        public virtual void DeleteNotification(NotificationInfo notification)
        {
            _unitOfWorkManager.WithUnitOfWork(() => { _notificationRepository.Delete(notification); });
        }

        public async Task<List<GetNotificationsCreatedByUserOutput>> GetNotificationsPublishedByUserAsync(
            UserIdentifier user, string notificationName, DateTime? startDate, DateTime? endDate)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(user.CompanyId))
                {
                    IQueryable<NotificationInfo> queryForNotPublishedNotifications = _notificationRepository.GetAll()
                        .Where(n => n.CreatorUserId == user.UserId && n.NotificationName == notificationName);

                    if (startDate.HasValue)
                    {
                        queryForNotPublishedNotifications = queryForNotPublishedNotifications
                            .Where(x => x.CreationTime >= startDate);
                    }

                    if (endDate.HasValue)
                    {
                        queryForNotPublishedNotifications = queryForNotPublishedNotifications
                            .Where(x => x.CreationTime <= endDate);
                    }

                    List<GetNotificationsCreatedByUserOutput> result = new List<GetNotificationsCreatedByUserOutput>();

                    List<GetNotificationsCreatedByUserOutput> unPublishedNotifications =
                        await AsyncQueryableExecuter.ToListAsync(queryForNotPublishedNotifications
                            .Select(x =>
                                new GetNotificationsCreatedByUserOutput
                                {
                                    Data = x.Data,
                                    Severity = x.Severity,
                                    NotificationName = x.NotificationName,
                                    DataTypeName = x.DataTypeName,
                                    IsPublished = false,
                                    CreationTime = x.CreationTime
                                })
                        );

                    result.AddRange(unPublishedNotifications);

                    IQueryable<CompanyNotificationInfo> queryForPublishedNotifications = _companyNotificationRepository
                        .GetAll()
                        .Where(n => n.CreatorUserId == user.UserId && n.NotificationName == notificationName);

                    if (startDate.HasValue)
                    {
                        queryForPublishedNotifications = queryForPublishedNotifications
                            .Where(x => x.CreationTime >= startDate);
                    }

                    if (endDate.HasValue)
                    {
                        queryForPublishedNotifications = queryForPublishedNotifications
                            .Where(x => x.CreationTime <= endDate);
                    }

                    queryForPublishedNotifications = queryForPublishedNotifications
                        .OrderByDescending(n => n.CreationTime);

                    List<GetNotificationsCreatedByUserOutput> publishedNotifications =
                        await AsyncQueryableExecuter.ToListAsync(queryForPublishedNotifications
                            .Select(x =>
                                new GetNotificationsCreatedByUserOutput
                                {
                                    Data = x.Data,
                                    Severity = x.Severity,
                                    NotificationName = x.NotificationName,
                                    DataTypeName = x.DataTypeName,
                                    IsPublished = true,
                                    CreationTime = x.CreationTime
                                })
                        );

                    result.AddRange(publishedNotifications);
                    return result;
                }
            });
        }

        protected virtual async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(
            int? companyId,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    return await _notificationSubscriptionRepository.GetAllListAsync(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }
            });
        }

        protected virtual List<NotificationSubscriptionInfo> GetSubscriptions(
            int? companyId,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    return _notificationSubscriptionRepository.GetAllList(s =>
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId
                    );
                }
            });
        }

        private Expression<Func<UserNotificationInfo, bool>> CreateNotificationFilterPredicate(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            ExpressionStarter<UserNotificationInfo> predicate = PredicateBuilder.New<UserNotificationInfo>();
            predicate = predicate.And(p => p.UserId == user.UserId);

            if (startDate.HasValue)
            {
                predicate = predicate.And(p => p.CreationTime >= startDate);
            }

            if (endDate.HasValue)
            {
                predicate = predicate.And(p => p.CreationTime <= endDate);
            }

            if (state.HasValue)
            {
                predicate = predicate.And(p => p.State == state);
            }

            return predicate;
        }
    }
}
