using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.BackgroundJobs;
using Kontecg.Collections.Extensions;
using Kontecg.Json;
using Kontecg.Webhooks.BackgroundWorker;

namespace Kontecg.Webhooks
{
    public class DefaultWebhookPublisher : ApplicationService, IWebhookPublisher
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;

        public DefaultWebhookPublisher(
            IWebhookSubscriptionManager webhookSubscriptionManager,
            IWebhooksConfiguration webhooksConfiguration,
            IGuidGenerator guidGenerator,
            IBackgroundJobManager backgroundJobManager)
        {
            _guidGenerator = guidGenerator;
            _backgroundJobManager = backgroundJobManager;
            _webhookSubscriptionManager = webhookSubscriptionManager;
            _webhooksConfiguration = webhooksConfiguration;

            WebhookEventStore = NullWebhookEventStore.Instance;
        }

        public IWebhookEventStore WebhookEventStore { get; set; }

        public virtual async Task PublishAsync(string webhookName, object data, bool sendExactSameData = false)
        {
            List<WebhookSubscription> subscriptions =
                await _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(KontecgSession.CompanyId,
                    webhookName);
            await PublishAsync(webhookName, data, subscriptions, sendExactSameData);
        }

        public virtual async Task PublishAsync(string webhookName, object data, int? companyId,
            bool sendExactSameData = false)
        {
            List<WebhookSubscription> subscriptions =
                await _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(companyId, webhookName);
            await PublishAsync(webhookName, data, subscriptions, sendExactSameData);
        }

        public virtual async Task PublishAsync(int?[] companyIds, string webhookName, object data,
            bool sendExactSameData = false)
        {
            List<WebhookSubscription> subscriptions =
                await _webhookSubscriptionManager.GetAllSubscriptionsOfCompanysIfFeaturesGrantedAsync(companyIds,
                    webhookName);
            await PublishAsync(webhookName, data, subscriptions, sendExactSameData);
        }

        public virtual void Publish(string webhookName, object data, bool sendExactSameData = false)
        {
            List<WebhookSubscription> subscriptions =
                _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(KontecgSession.CompanyId, webhookName);
            Publish(webhookName, data, subscriptions, sendExactSameData);
        }

        public virtual void Publish(string webhookName, object data, int? companyId, bool sendExactSameData = false)
        {
            List<WebhookSubscription> subscriptions =
                _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(companyId, webhookName);
            Publish(webhookName, data, subscriptions, sendExactSameData);
        }

        public virtual void Publish(int?[] companyIds, string webhookName, object data, bool sendExactSameData = false)
        {
            List<WebhookSubscription> subscriptions =
                _webhookSubscriptionManager.GetAllSubscriptionsOfCompanysIfFeaturesGranted(companyIds, webhookName);
            Publish(webhookName, data, subscriptions, sendExactSameData);
        }

        protected virtual async Task<WebhookEvent> SaveAndGetWebhookAsync(int? companyId, string webhookName,
            object data)
        {
            WebhookEvent webhookInfo = new WebhookEvent
            {
                Id = _guidGenerator.Create(),
                WebhookName = webhookName,
                Data = _webhooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString(),
                CompanyId = companyId
            };

            await WebhookEventStore.InsertAndGetIdAsync(webhookInfo);
            return webhookInfo;
        }

        protected virtual WebhookEvent SaveAndGetWebhook(int? companyId, string webhookName, object data)
        {
            WebhookEvent webhookInfo = new WebhookEvent
            {
                Id = _guidGenerator.Create(),
                WebhookName = webhookName,
                Data = _webhooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString(),
                CompanyId = companyId
            };

            WebhookEventStore.InsertAndGetId(webhookInfo);
            return webhookInfo;
        }

        private async Task PublishAsync(string webhookName, object data, List<WebhookSubscription> webhookSubscriptions,
            bool sendExactSameData = false)
        {
            if (webhookSubscriptions.IsNullOrEmpty())
            {
                return;
            }

            IEnumerable<IGrouping<int?, WebhookSubscription>> subscriptionsGroupedByCompany =
                webhookSubscriptions.GroupBy(x => x.CompanyId);

            foreach (IGrouping<int?, WebhookSubscription> subscriptionGroupedByCompany in subscriptionsGroupedByCompany)
            {
                WebhookEvent webhookInfo =
                    await SaveAndGetWebhookAsync(subscriptionGroupedByCompany.Key, webhookName, data);

                foreach (WebhookSubscription webhookSubscription in subscriptionGroupedByCompany)
                {
                    await _backgroundJobManager.EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(new WebhookSenderArgs
                    {
                        CompanyId = webhookSubscription.CompanyId,
                        WebhookEventId = webhookInfo.Id,
                        Data = webhookInfo.Data,
                        WebhookName = webhookInfo.WebhookName,
                        WebhookSubscriptionId = webhookSubscription.Id,
                        Headers = webhookSubscription.Headers,
                        Secret = webhookSubscription.Secret,
                        WebhookUri = webhookSubscription.WebhookUri,
                        SendExactSameData = sendExactSameData
                    });
                }
            }
        }

        private void Publish(string webhookName, object data, List<WebhookSubscription> webhookSubscriptions,
            bool sendExactSameData = false)
        {
            if (webhookSubscriptions.IsNullOrEmpty())
            {
                return;
            }

            IEnumerable<IGrouping<int?, WebhookSubscription>> subscriptionsGroupedByCompany =
                webhookSubscriptions.GroupBy(x => x.CompanyId);

            foreach (IGrouping<int?, WebhookSubscription> subscriptionGroupedByCompany in subscriptionsGroupedByCompany)
            {
                WebhookEvent webhookInfo = SaveAndGetWebhook(subscriptionGroupedByCompany.Key, webhookName, data);

                foreach (WebhookSubscription webhookSubscription in subscriptionGroupedByCompany)
                {
                    _backgroundJobManager.Enqueue<WebhookSenderJob, WebhookSenderArgs>(new WebhookSenderArgs
                    {
                        CompanyId = webhookSubscription.CompanyId,
                        WebhookEventId = webhookInfo.Id,
                        Data = webhookInfo.Data,
                        WebhookName = webhookInfo.WebhookName,
                        WebhookSubscriptionId = webhookSubscription.Id,
                        Headers = webhookSubscription.Headers,
                        Secret = webhookSubscription.Secret,
                        WebhookUri = webhookSubscription.WebhookUri,
                        SendExactSameData = sendExactSameData
                    });
                }
            }
        }
    }
}
