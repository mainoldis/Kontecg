using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Webhooks
{
    /// <summary>
    ///     Null pattern implementation of <see cref="IWebhookSubscriptionsStore" />.
    ///     It's used if <see cref="IWebhookSubscriptionsStore" /> is not implemented by actual persistent store
    /// </summary>
    public class NullWebhookSubscriptionsStore : IWebhookSubscriptionsStore
    {
        public static NullWebhookSubscriptionsStore Instance { get; } = new();

        public Task<WebhookSubscriptionInfo> GetAsync(Guid id)
        {
            return Task.FromResult<WebhookSubscriptionInfo>(default);
        }

        public WebhookSubscriptionInfo Get(Guid id)
        {
            return default;
        }

        public Task InsertAsync(WebhookSubscriptionInfo webhookSubscription)
        {
            return Task.CompletedTask;
        }

        public void Insert(WebhookSubscriptionInfo webhookSubscription)
        {
        }

        public Task UpdateAsync(WebhookSubscriptionInfo webhookSubscription)
        {
            return Task.CompletedTask;
        }

        public void Update(WebhookSubscriptionInfo webhookSubscription)
        {
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }

        public void Delete(Guid id)
        {
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? companyId)
        {
            return Task.FromResult(new List<WebhookSubscriptionInfo>());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptions(int? companyId)
        {
            return new List<WebhookSubscriptionInfo>();
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? companyId, string webhookName)
        {
            return Task.FromResult(new List<WebhookSubscriptionInfo>());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptions(int? companyId, string webhookName)
        {
            return new List<WebhookSubscriptionInfo>();
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfCompanysAsync(int?[] companyIds)
        {
            return Task.FromResult(new List<WebhookSubscriptionInfo>());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptionsOfCompanys(int?[] companyIds)
        {
            return new List<WebhookSubscriptionInfo>();
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfCompanysAsync(int?[] companyIds,
            string webhookName)
        {
            return Task.FromResult(new List<WebhookSubscriptionInfo>());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptionsOfCompanys(int?[] companyIds, string webhookName)
        {
            return new List<WebhookSubscriptionInfo>();
        }

        public Task<bool> IsSubscribedAsync(int? companyId, string webhookName)
        {
            return Task.FromResult(false);
        }

        public bool IsSubscribed(int? companyId, string webhookName)
        {
            return false;
        }
    }
}
