using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Webhooks
{
    public interface IWebhookSendAttemptStore
    {
        Task InsertAsync(WebhookSendAttempt webhookSendAttempt);

        void Insert(WebhookSendAttempt webhookSendAttempt);

        Task UpdateAsync(WebhookSendAttempt webhookSendAttempt);

        void Update(WebhookSendAttempt webhookSendAttempt);

        Task<WebhookSendAttempt> GetAsync(int? companyId, Guid id);

        WebhookSendAttempt Get(int? companyId, Guid id);

        /// <summary>
        ///     Returns work item count by given web hook id and subscription id, (How many times publisher tried to send web hook)
        /// </summary>
        Task<int> GetSendAttemptCountAsync(int? companyId, Guid webhookId, Guid webhookSubscriptionId);

        /// <summary>
        ///     Returns work item count by given web hook id and subscription id. (How many times publisher tried to send web hook)
        /// </summary>
        int GetSendAttemptCount(int? companyId, Guid webhookId, Guid webhookSubscriptionId);

        /// <summary>
        ///     Checks is there any successful webhook attempt in last <paramref name="searchCount" /> items. Should return true if
        ///     there are not X number items
        /// </summary>
        Task<bool> HasXConsecutiveFailAsync(int? companyId, Guid subscriptionId, int searchCount);

        Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(int? companyId,
            Guid subscriptionId, int maxResultCount, int skipCount);

        IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(int? companyId,
            Guid subscriptionId,
            int maxResultCount, int skipCount);

        Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(int? companyId, Guid webhookEventId);

        List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(int? companyId, Guid webhookEventId);
    }
}
