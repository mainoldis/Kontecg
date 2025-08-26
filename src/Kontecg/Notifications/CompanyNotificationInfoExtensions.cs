using System;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;
using Newtonsoft.Json;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Extension methods for <see cref="NotificationInfo" />.
    /// </summary>
    public static class CompanyNotificationInfoExtensions
    {
        /// <summary>
        ///     Converts <see cref="NotificationInfo" /> to <see cref="CompanyNotification" />.
        /// </summary>
        public static CompanyNotification ToCompanyNotification(this CompanyNotificationInfo companyNotificationInfo)
        {
            Type entityType = companyNotificationInfo.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
                ? null
                : Type.GetType(companyNotificationInfo.EntityTypeAssemblyQualifiedName);

            return new CompanyNotification
            {
                Id = companyNotificationInfo.Id,
                CompanyId = companyNotificationInfo.CompanyId,
                NotificationName = companyNotificationInfo.NotificationName,
                Data = companyNotificationInfo.Data.IsNullOrEmpty()
                    ? null
                    : JsonConvert.DeserializeObject(companyNotificationInfo.Data,
                        Type.GetType(companyNotificationInfo.DataTypeName)) as NotificationData,
                EntityTypeName = companyNotificationInfo.EntityTypeName,
                EntityType = entityType,
                EntityId = companyNotificationInfo.EntityId.IsNullOrEmpty()
                    ? null
                    : JsonConvert.DeserializeObject(companyNotificationInfo.EntityId,
                        EntityHelper.GetPrimaryKeyType(entityType)),
                Severity = companyNotificationInfo.Severity,
                CreationTime = companyNotificationInfo.CreationTime
            };
        }
    }
}
