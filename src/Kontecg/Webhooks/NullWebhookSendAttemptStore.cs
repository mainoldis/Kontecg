using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Webhooks
{
    public class NullWebhookSendAttemptStore : IWebhookSendAttemptStore
    {
        public static NullWebhookSendAttemptStore Instance = new();

        public Task InsertAsync(WebhookSendAttempt webhookSendAttempt)
        {
            return Task.CompletedTask;
        }

        public void Insert(WebhookSendAttempt webhookSendAttempt)
        {
        }

        public Task UpdateAsync(WebhookSendAttempt webhookSendAttempt)
        {
            return Task.CompletedTask;
        }

        public void Update(WebhookSendAttempt webhookSendAttempt)
        {
        }

        public Task<WebhookSendAttempt> GetAsync(int? companyId, Guid id)
        {
            return Task.FromResult<WebhookSendAttempt>(default);
        }

        public WebhookSendAttempt Get(int? companyId, Guid id)
        {
            return default;
        }

        public Task<int> GetSendAttemptCountAsync(int? companyId, Guid webhookId, Guid webhookSubscriptionId)
        {
            return Task.FromResult(int.MaxValue);
        }

        public int GetSendAttemptCount(int? companyId, Guid webhookId, Guid webhookSubscriptionId)
        {
            return int.MaxValue;
        }

        public Task<bool> HasXConsecutiveFailAsync(int? companyId, Guid subscriptionId, int searchCount)
        {
            return Task.FromResult(default(bool));
        }

        public Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(int? companyId,
            Guid subscriptionId, int maxResultCount,
            int skipCount)
        {
            return Task.FromResult(new PagedResultDto<WebhookSendAttempt>() as IPagedResult<WebhookSendAttempt>);
        }

        public IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(int? companyId,
            Guid subscriptionId, int maxResultCount,
            int skipCount)
        {
            return new PagedResultDto<WebhookSendAttempt>();
        }

        public Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(int? companyId,
            Guid webhookEventId)
        {
            return Task.FromResult(new List<WebhookSendAttempt>());
        }

        public List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(int? companyId, Guid webhookEventId)
        {
            return new List<WebhookSendAttempt>();
        }
    }
}
