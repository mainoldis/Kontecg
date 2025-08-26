using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Used to distribute notifications to users.
    /// </summary>
    public class DefaultNotificationDistributer : DomainService, INotificationDistributer
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IIocResolver _iocResolver;
        private readonly INotificationConfiguration _notificationConfiguration;
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly INotificationStore _notificationStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationDistributionJob" /> class.
        /// </summary>
        public DefaultNotificationDistributer(
            INotificationConfiguration notificationConfiguration,
            INotificationDefinitionManager notificationDefinitionManager,
            INotificationStore notificationStore,
            IUnitOfWorkManager unitOfWorkManager,
            IGuidGenerator guidGenerator,
            IIocResolver iocResolver)
        {
            _notificationConfiguration = notificationConfiguration;
            _notificationDefinitionManager = notificationDefinitionManager;
            _notificationStore = notificationStore;
            _unitOfWorkManager = unitOfWorkManager;
            _guidGenerator = guidGenerator;
            _iocResolver = iocResolver;
        }

        public virtual async Task DistributeAsync(Guid notificationId)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                NotificationInfo notificationInfo = await _notificationStore.GetNotificationOrNullAsync(notificationId);
                if (notificationInfo == null)
                {
                    Logger.Warn(
                        "NotificationDistributionJob can not continue since could not found notification by id: " +
                        notificationId);
                    return;
                }

                UserIdentifier[] users = await GetUsersAsync(notificationInfo);

                List<UserNotification> userNotifications = await SaveUserNotificationsAsync(users, notificationInfo);

                await _notificationStore.DeleteNotificationAsync(notificationInfo);

                await NotifyAsync(userNotifications.ToArray());
            });
        }

        protected virtual async Task<UserIdentifier[]> GetUsersAsync(NotificationInfo notificationInfo)
        {
            List<UserIdentifier> userIds;

            if (!notificationInfo.UserIds.IsNullOrEmpty())
            {
                //Directly get from UserIds
                userIds = notificationInfo
                    .UserIds
                    .Split(",")
                    .Select(UserIdentifier.Parse)
                    .Where(uid =>
                        SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications,
                            uid.CompanyId, uid.UserId))
                    .ToList();
            }
            else
            {
                //Get subscribed users

                int?[] companyIds = GetCompanyIds(notificationInfo);

                List<NotificationSubscriptionInfo> subscriptions;

                if (companyIds.IsNullOrEmpty() ||
                    (companyIds.Length == 1 && companyIds[0] == NotificationInfo.AllCompanyIds.To<int>()))
                    //Get all subscribed users of all companies
                {
                    subscriptions = await _notificationStore.GetSubscriptionsAsync(
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                    );
                }
                else
                    //Get all subscribed users of specified company(s)
                {
                    subscriptions = await _notificationStore.GetSubscriptionsAsync(
                        companyIds,
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                    );
                }

                //Remove invalid subscriptions
                Dictionary<Guid, NotificationSubscriptionInfo> invalidSubscriptions =
                    new Dictionary<Guid, NotificationSubscriptionInfo>();

                //TODO: Group subscriptions per company for potential performance improvement
                foreach (NotificationSubscriptionInfo subscription in subscriptions)
                {
                    using (CurrentUnitOfWork.SetCompanyId(subscription.CompanyId))
                    {
                        if (!await _notificationDefinitionManager.IsAvailableAsync(notificationInfo.NotificationName,
                                new UserIdentifier(subscription.CompanyId, subscription.UserId)) ||
                            !SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications,
                                subscription.CompanyId, subscription.UserId))
                        {
                            invalidSubscriptions[subscription.Id] = subscription;
                        }
                    }
                }

                subscriptions.RemoveAll(s => invalidSubscriptions.ContainsKey(s.Id));

                //Get user ids
                userIds = subscriptions
                    .Select(s => new UserIdentifier(s.CompanyId, s.UserId))
                    .ToList();
            }

            if (!notificationInfo.ExcludedUserIds.IsNullOrEmpty())
            {
                //Exclude specified users.
                List<UserIdentifier> excludedUserIds = notificationInfo
                    .ExcludedUserIds
                    .Split(",")
                    .Select(UserIdentifier.Parse)
                    .ToList();

                userIds.RemoveAll(uid => excludedUserIds.Any(euid => euid.Equals(uid)));
            }

            return userIds.ToArray();
        }

        protected virtual UserIdentifier[] GetUsers(NotificationInfo notificationInfo)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                List<UserIdentifier> userIds;

                if (!notificationInfo.UserIds.IsNullOrEmpty())
                {
                    //Directly get from UserIds
                    userIds = notificationInfo
                        .UserIds
                        .Split(",")
                        .Select(UserIdentifier.Parse)
                        .Where(uid =>
                            SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications,
                                uid.CompanyId, uid.UserId))
                        .ToList();
                }
                else
                {
                    //Get subscribed users

                    int?[] companyIds = GetCompanyIds(notificationInfo);

                    List<NotificationSubscriptionInfo> subscriptions;

                    if (companyIds.IsNullOrEmpty() ||
                        (companyIds.Length == 1 && companyIds[0] == NotificationInfo.AllCompanyIds.To<int>()))
                        //Get all subscribed users of all companies
                    {
                        subscriptions = _notificationStore.GetSubscriptions(
                            notificationInfo.NotificationName,
                            notificationInfo.EntityTypeName,
                            notificationInfo.EntityId
                        );
                    }
                    else
                        //Get all subscribed users of specified company(s)
                    {
                        subscriptions = _notificationStore.GetSubscriptions(
                            companyIds,
                            notificationInfo.NotificationName,
                            notificationInfo.EntityTypeName,
                            notificationInfo.EntityId
                        );
                    }

                    //Remove invalid subscriptions
                    Dictionary<Guid, NotificationSubscriptionInfo> invalidSubscriptions =
                        new Dictionary<Guid, NotificationSubscriptionInfo>();

                    //TODO: Group subscriptions per company for potential performance improvement
                    foreach (NotificationSubscriptionInfo subscription in subscriptions)
                    {
                        using (CurrentUnitOfWork.SetCompanyId(subscription.CompanyId))
                        {
                            if (!_notificationDefinitionManager.IsAvailable(notificationInfo.NotificationName,
                                    new UserIdentifier(subscription.CompanyId, subscription.UserId)) ||
                                !SettingManager.GetSettingValueForUser<bool>(
                                    NotificationSettingNames.ReceiveNotifications, subscription.CompanyId,
                                    subscription.UserId))
                            {
                                invalidSubscriptions[subscription.Id] = subscription;
                            }
                        }
                    }

                    subscriptions.RemoveAll(s => invalidSubscriptions.ContainsKey(s.Id));

                    //Get user ids
                    userIds = subscriptions
                        .Select(s => new UserIdentifier(s.CompanyId, s.UserId))
                        .ToList();
                }

                if (!notificationInfo.ExcludedUserIds.IsNullOrEmpty())
                {
                    //Exclude specified users.
                    List<UserIdentifier> excludedUserIds = notificationInfo
                        .ExcludedUserIds
                        .Split(",")
                        .Select(UserIdentifier.Parse)
                        .ToList();

                    userIds.RemoveAll(uid => excludedUserIds.Any(euid => euid.Equals(uid)));
                }

                return userIds.ToArray();
            });
        }

        protected virtual async Task<List<UserNotification>> SaveUserNotificationsAsync(UserIdentifier[] users,
            NotificationInfo notificationInfo)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                List<UserNotification> userNotifications = new List<UserNotification>();

                IEnumerable<IGrouping<int?, UserIdentifier>> companyGroups = users.GroupBy(user => user.CompanyId);
                foreach (IGrouping<int?, UserIdentifier> companyGroup in companyGroups)
                {
                    using (_unitOfWorkManager.Current.SetCompanyId(companyGroup.Key))
                    {
                        CompanyNotificationInfo companyNotificationInfo = new CompanyNotificationInfo(
                            _guidGenerator.Create(),
                            companyGroup.Key, notificationInfo);
                        await _notificationStore.InsertCompanyNotificationAsync(companyNotificationInfo);
                        await _unitOfWorkManager.Current.SaveChangesAsync(); //To get companyNotification.Id.

                        CompanyNotification companyNotification = companyNotificationInfo.ToCompanyNotification();

                        foreach (UserIdentifier user in companyGroup)
                        {
                            UserNotificationInfo userNotification = new UserNotificationInfo(_guidGenerator.Create())
                            {
                                CompanyId = companyGroup.Key,
                                UserId = user.UserId,
                                CompanyNotificationId = companyNotificationInfo.Id
                            };

                            await _notificationStore.InsertUserNotificationAsync(userNotification);
                            userNotifications.Add(userNotification.ToUserNotification(companyNotification));
                        }

                        await CurrentUnitOfWork.SaveChangesAsync(); //To get Ids of the notifications
                    }
                }

                return userNotifications;
            });
        }

        protected virtual List<UserNotification> SaveUserNotifications(
            UserIdentifier[] users,
            NotificationInfo notificationInfo)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                List<UserNotification> userNotifications = new List<UserNotification>();

                IEnumerable<IGrouping<int?, UserIdentifier>> companyGroups = users.GroupBy(user => user.CompanyId);
                foreach (IGrouping<int?, UserIdentifier> companyGroup in companyGroups)
                {
                    using (_unitOfWorkManager.Current.SetCompanyId(companyGroup.Key))
                    {
                        CompanyNotificationInfo companyNotificationInfo = new CompanyNotificationInfo(
                            _guidGenerator.Create(),
                            companyGroup.Key, notificationInfo);
                        _notificationStore.InsertCompanyNotification(companyNotificationInfo);
                        _unitOfWorkManager.Current.SaveChanges(); //To get companyNotification.Id.

                        CompanyNotification companyNotification = companyNotificationInfo.ToCompanyNotification();

                        foreach (UserIdentifier user in companyGroup)
                        {
                            UserNotificationInfo userNotification = new UserNotificationInfo(_guidGenerator.Create())
                            {
                                CompanyId = companyGroup.Key,
                                UserId = user.UserId,
                                CompanyNotificationId = companyNotificationInfo.Id
                            };

                            _notificationStore.InsertUserNotification(userNotification);
                            userNotifications.Add(userNotification.ToUserNotification(companyNotification));
                        }

                        CurrentUnitOfWork.SaveChanges(); //To get Ids of the notifications
                    }
                }

                return userNotifications;
            });
        }

        #region Protected methods

        protected virtual async Task NotifyAsync(UserNotification[] userNotifications)
        {
            foreach (Type notifierType in _notificationConfiguration.Notifiers)
            {
                try
                {
                    using IDisposableDependencyObjectWrapper<IRealTimeNotifier> notifier =
                        _iocResolver.ResolveAsDisposable<IRealTimeNotifier>(notifierType);
                    await notifier.Object.SendNotificationsAsync(userNotifications);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }
        }

        #endregion

        private static int?[] GetCompanyIds(NotificationInfo notificationInfo)
        {
            if (notificationInfo.CompanyIds.IsNullOrEmpty())
            {
                return null;
            }

            return notificationInfo
                .CompanyIds
                .Split(",")
                .Select(companyIdAsStr => companyIdAsStr == "null" ? null : (int?) companyIdAsStr.To<int>())
                .ToArray();
        }
    }
}
