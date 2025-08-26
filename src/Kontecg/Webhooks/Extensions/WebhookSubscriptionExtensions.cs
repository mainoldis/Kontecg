using System.Linq;
using Kontecg.Collections.Extensions;

namespace Kontecg.Webhooks.Extensions
{
    public static class WebhookSubscriptionExtensions
    {
        /// <summary>
        ///     checks if subscribed to given webhook
        /// </summary>
        /// <returns></returns>
        public static bool IsSubscribed(this WebhookSubscription webhookSubscription, string webhookName)
        {
            if (webhookSubscription.Webhooks.IsNullOrEmpty())
            {
                return false;
            }

            return webhookSubscription.Webhooks.Contains(webhookName);
        }

        public static WebhookSubscription ToWebhookSubscription(this WebhookSubscriptionInfo webhookSubscriptionInfo)
        {
            return new WebhookSubscription
            {
                Id = webhookSubscriptionInfo.Id,
                CompanyId = webhookSubscriptionInfo.CompanyId,
                IsActive = webhookSubscriptionInfo.IsActive,
                Secret = webhookSubscriptionInfo.Secret,
                WebhookUri = webhookSubscriptionInfo.WebhookUri,
                Webhooks = webhookSubscriptionInfo.GetSubscribedWebhooks().ToList(),
                Headers = webhookSubscriptionInfo.GetWebhookHeaders()
            };
        }
    }
}
