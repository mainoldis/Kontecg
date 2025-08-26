﻿using System;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;
using Newtonsoft.Json;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Extension methods for <see cref="NotificationSubscriptionInfo" />.
    /// </summary>
    public static class NotificationSubscriptionInfoExtensions
    {
        /// <summary>
        ///     Converts <see cref="UserNotificationInfo" /> to <see cref="UserNotification" />.
        /// </summary>
        public static NotificationSubscription ToNotificationSubscription(
            this NotificationSubscriptionInfo subscriptionInfo)
        {
            Type entityType = subscriptionInfo.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
                ? null
                : Type.GetType(subscriptionInfo.EntityTypeAssemblyQualifiedName);

            return new NotificationSubscription
            {
                CompanyId = subscriptionInfo.CompanyId,
                UserId = subscriptionInfo.UserId,
                NotificationName = subscriptionInfo.NotificationName,
                EntityType = entityType,
                EntityTypeName = subscriptionInfo.EntityTypeName,
                EntityId = subscriptionInfo.EntityId.IsNullOrEmpty()
                    ? null
                    : JsonConvert.DeserializeObject(subscriptionInfo.EntityId,
                        EntityHelper.GetPrimaryKeyType(entityType)),
                CreationTime = subscriptionInfo.CreationTime
            };
        }
    }
}
