using System.Threading.Tasks;
using Kontecg.BackgroundJobs;
using Kontecg.Domain.Uow;
using Kontecg.Notifications;
using NSubstitute;
using Xunit;

namespace Kontecg.Tests.Notifications
{
    public class NotificationPublisher_Tests : TestBaseWithLocalIocManager
    {
        private readonly NotificationPublisher _publisher;
        private readonly INotificationStore _store;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public NotificationPublisher_Tests()
        {
            _store = Substitute.For<INotificationStore>();
            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            _publisher = new NotificationPublisher(
                _store,
                _backgroundJobManager,
                Substitute.For<INotificationDistributer>(),
                UuidGenerator.Instance, Substitute.For<INotificationConfiguration>())
            {
                UnitOfWorkManager = Substitute.For<IUnitOfWorkManager>()
            };

            _publisher.UnitOfWorkManager.Current.Returns(Substitute.For<IActiveUnitOfWork>());
        }

        [Fact]
        public async Task Should_Publish_General_Notification()
        {
            //Arrange
            var notificationData = CreateNotificationData();

            //Act
            await _publisher.PublishAsync("TestNotification", notificationData, severity: NotificationSeverity.Success);

            //Assert
            await _store.Received()
                .InsertNotificationAsync(
                    Arg.Is<NotificationInfo>(
                        n => n.NotificationName == "TestNotification" &&
                             n.Severity == NotificationSeverity.Success &&
                             n.DataTypeName == notificationData.GetType().AssemblyQualifiedName &&
                             n.Data.Contains("42")
                        )
                );

            await _backgroundJobManager.Received()
                .EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                    Arg.Any<NotificationDistributionJobArgs>()
                );
        }

        [Fact]
        public async Task Should_PublishAsync_To_Host()
        {
            // Act
            await _publisher.PublishAsync("TestNotification", companyIds: new int?[] { null });

            // Assert
            await _store.Received()
                .InsertNotificationAsync(
                    Arg.Is<NotificationInfo>(n => n.CompanyIds == "null")
                );
        }

        private static NotificationData CreateNotificationData()
        {
            var notificationData = new NotificationData
            {
                ["TestValue"] = 42
            };
            
            return notificationData;
        }
    }
}
