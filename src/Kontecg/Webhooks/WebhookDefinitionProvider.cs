using Kontecg.Dependency;

namespace Kontecg.Webhooks
{
    public abstract class WebhookDefinitionProvider : ITransientDependency
    {
        /// <summary>
        ///     Used to add/manipulate webhook definitions.
        /// </summary>
        /// <param name="context">Context</param>
        /// ,
        public abstract void SetWebhooks(IWebhookDefinitionContext context);
    }
}
