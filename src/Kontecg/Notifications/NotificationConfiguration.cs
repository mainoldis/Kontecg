using Kontecg.Collections;

namespace Kontecg.Notifications
{
    internal class NotificationConfiguration : INotificationConfiguration
    {
        public NotificationConfiguration()
        {
            Providers = new TypeList<NotificationProvider>();
            Distributers = new TypeList<INotificationDistributer>();
            Notifiers = new TypeList<IRealTimeNotifier>();
        }

        public ITypeList<INotificationDistributer> Distributers { get; }
        public ITypeList<NotificationProvider> Providers { get; }

        public ITypeList<IRealTimeNotifier> Notifiers { get; }
    }
}
