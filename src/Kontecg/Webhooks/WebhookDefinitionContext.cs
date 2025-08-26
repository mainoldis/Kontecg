namespace Kontecg.Webhooks
{
    public class WebhookDefinitionContext : IWebhookDefinitionContext
    {
        public WebhookDefinitionContext(IWebhookDefinitionManager manager)
        {
            Manager = manager;
        }

        public IWebhookDefinitionManager Manager { get; }
    }
}
