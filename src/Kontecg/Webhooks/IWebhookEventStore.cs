using System;
using System.Threading.Tasks;

namespace Kontecg.Webhooks
{
    public interface IWebhookEventStore
    {
        /// <summary>
        ///     Inserts to persistent store
        /// </summary>
        Task<Guid> InsertAndGetIdAsync(WebhookEvent webhookEvent);

        /// <summary>
        ///     Inserts to persistent store
        /// </summary>
        Guid InsertAndGetId(WebhookEvent webhookEvent);

        /// <summary>
        ///     Gets Webhook info by id
        /// </summary>
        Task<WebhookEvent> GetAsync(int? companyId, Guid id);

        /// <summary>
        ///     Gets Webhook info by id
        /// </summary>
        WebhookEvent Get(int? companyId, Guid id);
    }
}
