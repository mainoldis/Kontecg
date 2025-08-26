using System;
using Kontecg.Collections;
using Newtonsoft.Json;

namespace Kontecg.Webhooks
{
    internal class WebhooksConfiguration : IWebhooksConfiguration
    {
        public WebhooksConfiguration()
        {
            Providers = new TypeList<WebhookDefinitionProvider>();

            ConsecutiveFailCountBeforeDeactivateSubscription =
                SendAttemptCount * 3; //deactivates subscription if 3 webhook can not be sent.
        }

        public TimeSpan TimeoutDuration { get; set; } = TimeSpan.FromSeconds(60);

        public int SendAttemptCount { get; set; } = 5;

        public ITypeList<WebhookDefinitionProvider> Providers { get; }

        public JsonSerializerSettings JsonSerializerSettings { get; set; } = null;

        public bool IsAutomaticSubscriptionDeactivationEnabled { get; set; } = false;

        public int ConsecutiveFailCountBeforeDeactivateSubscription { get; set; }
    }
}
