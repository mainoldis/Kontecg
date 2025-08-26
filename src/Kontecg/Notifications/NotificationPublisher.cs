using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.BackgroundJobs;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.Json;
using Kontecg.Runtime.Session;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Implements <see cref="INotificationPublisher" />.
    /// </summary>
    public class NotificationPublisher : KontecgServiceBase, INotificationPublisher, ITransientDependency
    {
        public const int MaxUserCountToDirectlyDistributeANotification = 5;

        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly INotificationConfiguration _notificationConfiguration;
        private readonly INotificationDistributer _notificationDistributer;
        private readonly INotificationStore _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationPublisher" /> class.
        /// </summary>
        public NotificationPublisher(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager,
            INotificationDistributer notificationDistributer,
            IGuidGenerator guidGenerator,
            INotificationConfiguration notificationConfiguration)
        {
            _store = store;
            _backgroundJobManager = backgroundJobManager;
            _notificationDistributer = notificationDistributer;
            _guidGenerator = guidGenerator;
            _notificationConfiguration = notificationConfiguration;
            KontecgSession = NullKontecgSession.Instance;
        }

        /// <summary>
        ///     Indicates all companies.
        /// </summary>
        public static int[] AllCompanies => new[] {NotificationInfo.AllCompanyIds.To<int>()};

        /// <summary>
        ///     Reference to Kontecg session.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        public virtual async Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            int?[] companyIds = null,
            Type[] targetNotifiers = null)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            if (notificationName.IsNullOrEmpty())
            {
                throw new ArgumentException("NotificationName can not be null or whitespace!",
                    nameof(notificationName));
            }

            if (!companyIds.IsNullOrEmpty() && !userIds.IsNullOrEmpty())
            {
                throw new ArgumentException("companyIds can be set only if userIds is not set!", nameof(companyIds));
            }

            if (companyIds.IsNullOrEmpty() && userIds.IsNullOrEmpty())
            {
                companyIds = new[] {KontecgSession.CompanyId};
            }

            NotificationInfo notificationInfo = new NotificationInfo(_guidGenerator.Create())
            {
                NotificationName = notificationName,
                EntityTypeName = entityIdentifier?.Type.FullName,
                EntityTypeAssemblyQualifiedName = entityIdentifier?.Type.AssemblyQualifiedName,
                EntityId = entityIdentifier?.Id.ToJsonString(),
                Severity = severity,
                UserIds = userIds.IsNullOrEmpty()
                    ? null
                    : userIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                ExcludedUserIds = excludedUserIds.IsNullOrEmpty()
                    ? null
                    : excludedUserIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                CompanyIds = GetCompanyIdsAsStr(companyIds),
                Data = data?.ToJsonString(),
                DataTypeName = data?.GetType().AssemblyQualifiedName
            };

            SetTargetNotifiers(notificationInfo, targetNotifiers);

            await _store.InsertNotificationAsync(notificationInfo);

            await CurrentUnitOfWork.SaveChangesAsync(); //To get Id of the notification

            if (userIds is {Length: <= MaxUserCountToDirectlyDistributeANotification})
                //We can directly distribute the notification since there are not much receivers
            {
                await _notificationDistributer.DistributeAsync(notificationInfo.Id);
            }
            else
                //We enqueue a background job since distributing may get a long time
            {
                await _backgroundJobManager
                    .EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                        new NotificationDistributionJobArgs(
                            notificationInfo.Id
                        )
                    );
            }

            await uow.CompleteAsync();
        }

        protected virtual void SetTargetNotifiers(NotificationInfo notificationInfo, Type[] targetNotifiers)
        {
            if (targetNotifiers == null)
            {
                return;
            }

            List<string> allNotificationNotifiers =
                _notificationConfiguration.Notifiers.Select(notifier => notifier.FullName).ToList();

            foreach (Type targetNotifier in targetNotifiers)
            {
                if (!allNotificationNotifiers.Contains(targetNotifier.FullName))
                {
                    throw new ApplicationException("Given target notifier is not registered before: " +
                                                   targetNotifier.FullName +
                                                   " You must register it to the INotificationConfiguration.Notifiers!");
                }
            }

            notificationInfo.SetTargetNotifiers(targetNotifiers.Select(n => n.FullName).ToList());
        }

        /// <summary>
        ///     Gets the string for <see cref="NotificationInfo.CompanyIds" />.
        /// </summary>
        /// <param name="companyIds"></param>
        /// <seealso cref="DefaultNotificationDistributer.GetCompanyIds" />
        private static string GetCompanyIdsAsStr(int?[] companyIds)
        {
            if (companyIds.IsNullOrEmpty())
            {
                return null;
            }

            return companyIds
                .Select(companyId => companyId == null ? "null" : companyId.ToString())
                .JoinAsString(",");
        }
    }
}
