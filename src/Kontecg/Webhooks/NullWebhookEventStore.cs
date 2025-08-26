using System;
using System.Threading.Tasks;

namespace Kontecg.Webhooks
{
    /// <summary>
    ///     Null pattern implementation of <see cref="IWebhookSubscriptionsStore" />.
    ///     It's used if <see cref="IWebhookSubscriptionsStore" /> is not implemented by actual persistent store
    /// </summary>
    public class NullWebhookEventStore : IWebhookEventStore
    {
        public static NullWebhookEventStore Instance { get; } = new();

        public Task<Guid> InsertAndGetIdAsync(WebhookEvent webhookEvent)
        {
            return Task.FromResult<Guid>(default);
        }

        public Guid InsertAndGetId(WebhookEvent webhookEvent)
        {
            return default;
        }

        public Task<WebhookEvent> GetAsync(int? companyId, Guid id)
        {
            return Task.FromResult<WebhookEvent>(default);
        }

        public WebhookEvent Get(int? companyId, Guid id)
        {
            return default;
        }
    }
}
