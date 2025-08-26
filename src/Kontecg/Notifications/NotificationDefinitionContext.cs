namespace Kontecg.Notifications
{
    internal class NotificationDefinitionContext : INotificationDefinitionContext
    {
        public NotificationDefinitionContext(INotificationDefinitionManager manager)
        {
            Manager = manager;
        }

        public INotificationDefinitionManager Manager { get; }
    }
}
