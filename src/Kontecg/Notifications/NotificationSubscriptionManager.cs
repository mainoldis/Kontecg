using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Json;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Implements <see cref="INotificationSubscriptionManager" />.
    /// </summary>
    public class NotificationSubscriptionManager : INotificationSubscriptionManager, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly INotificationStore _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationSubscriptionManager" /> class.
        /// </summary>
        public NotificationSubscriptionManager(
            INotificationStore store,
            INotificationDefinitionManager notificationDefinitionManager,
            IGuidGenerator guidGenerator)
        {
            _store = store;
            _notificationDefinitionManager = notificationDefinitionManager;
            _guidGenerator = guidGenerator;
        }

        public async Task SubscribeAsync(UserIdentifier user, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            if (await IsSubscribedAsync(user, notificationName, entityIdentifier))
            {
                return;
            }

            await _store.InsertSubscriptionAsync(
                new NotificationSubscriptionInfo(
                    _guidGenerator.Create(),
                    user.CompanyId,
                    user.UserId,
                    notificationName,
                    entityIdentifier
                )
            );
        }

        public void Subscribe(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            if (IsSubscribed(user, notificationName, entityIdentifier))
            {
                return;
            }

            _store.InsertSubscription(
                new NotificationSubscriptionInfo(
                    _guidGenerator.Create(),
                    user.CompanyId,
                    user.UserId,
                    notificationName,
                    entityIdentifier
                )
            );
        }

        public async Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user)
        {
            List<NotificationDefinition> notificationDefinitions = (await _notificationDefinitionManager
                    .GetAllAvailableAsync(user))
                .Where(nd => nd.EntityType == null)
                .ToList();

            foreach (NotificationDefinition notificationDefinition in notificationDefinitions)
            {
                await SubscribeAsync(user, notificationDefinition.Name);
            }
        }

        public void SubscribeToAllAvailableNotifications(UserIdentifier user)
        {
            List<NotificationDefinition> notificationDefinitions = _notificationDefinitionManager
                .GetAllAvailable(user)
                .Where(nd => nd.EntityType == null)
                .ToList();

            foreach (NotificationDefinition notificationDefinition in notificationDefinitions)
            {
                Subscribe(user, notificationDefinition.Name);
            }
        }

        public async Task UnsubscribeAsync(UserIdentifier user, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            await _store.DeleteSubscriptionAsync(
                user,
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );
        }

        public void Unsubscribe(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            _store.DeleteSubscription(
                user,
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );
        }

        // TODO: Can work only for single database approach!
        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptionInfos = await _store.GetSubscriptionsAsync(
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        // TODO: Can work only for single database approach!
        public List<NotificationSubscription> GetSubscriptions(string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptionInfos = _store.GetSubscriptions(
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(int? companyId, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptionInfos = await _store.GetSubscriptionsAsync(
                new[] {companyId},
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public List<NotificationSubscription> GetSubscriptions(int? companyId, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptionInfos = _store.GetSubscriptions(
                new[] {companyId},
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscribedNotificationsAsync(UserIdentifier user)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptionInfos = await _store.GetSubscriptionsAsync(user);

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public List<NotificationSubscription> GetSubscribedNotifications(UserIdentifier user)
        {
            List<NotificationSubscriptionInfo> notificationSubscriptionInfos = _store.GetSubscriptions(user);

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            return _store.IsSubscribedAsync(
                user,
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );
        }

        public bool IsSubscribed(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            return _store.IsSubscribed(
                user,
                notificationName,
                entityIdentifier?.Type.FullName,
                entityIdentifier?.Id.ToJsonString()
            );
        }
    }
}
