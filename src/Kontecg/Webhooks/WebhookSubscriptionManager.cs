using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Json;
using Kontecg.Webhooks.Extensions;

namespace Kontecg.Webhooks
{
    public class WebhookSubscriptionManager : DomainService, IWebhookSubscriptionManager
    {
        private const string WebhookSubscriptionSecretPrefix = "whs_";

        private readonly IGuidGenerator _guidGenerator;

        private readonly IWebhookDefinitionManager _webhookDefinitionManager;

        public WebhookSubscriptionManager(
            IGuidGenerator guidGenerator,
            IWebhookDefinitionManager webhookDefinitionManager)
        {
            _guidGenerator = guidGenerator;
            _webhookDefinitionManager = webhookDefinitionManager;

            WebhookSubscriptionsStore = NullWebhookSubscriptionsStore.Instance;
        }

        public IWebhookSubscriptionsStore WebhookSubscriptionsStore { get; set; }

        public virtual async Task<WebhookSubscription> GetAsync(Guid id)
        {
            return (await WebhookSubscriptionsStore.GetAsync(id)).ToWebhookSubscription();
        }

        public virtual WebhookSubscription Get(Guid id)
        {
            return WebhookSubscriptionsStore.Get(id).ToWebhookSubscription();
        }

        public virtual async Task<List<WebhookSubscription>> GetAllSubscriptionsAsync(int? companyId)
        {
            return (await WebhookSubscriptionsStore.GetAllSubscriptionsAsync(companyId))
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual List<WebhookSubscription> GetAllSubscriptions(int? companyId)
        {
            return WebhookSubscriptionsStore.GetAllSubscriptions(companyId)
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual async Task<List<WebhookSubscription>> GetAllSubscriptionsIfFeaturesGrantedAsync(int? companyId,
            string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(companyId, webhookName))
            {
                return new List<WebhookSubscription>();
            }

            return (await WebhookSubscriptionsStore.GetAllSubscriptionsAsync(companyId, webhookName))
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual List<WebhookSubscription> GetAllSubscriptionsIfFeaturesGranted(int? companyId,
            string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(companyId, webhookName))
            {
                return new List<WebhookSubscription>();
            }

            return WebhookSubscriptionsStore.GetAllSubscriptions(companyId, webhookName)
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual async Task<List<WebhookSubscription>> GetAllSubscriptionsOfCompanysAsync(int?[] companyIds)
        {
            return (await WebhookSubscriptionsStore.GetAllSubscriptionsOfCompanysAsync(companyIds))
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual List<WebhookSubscription> GetAllSubscriptionsOfCompanys(int?[] companyIds)
        {
            return WebhookSubscriptionsStore.GetAllSubscriptionsOfCompanys(companyIds)
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual async Task<List<WebhookSubscription>> GetAllSubscriptionsOfCompanysIfFeaturesGrantedAsync(
            int?[] companyIds, string webhookName)
        {
            List<int?> featureGrantedCompanys = new List<int?>();
            foreach (int? companyId in companyIds)
            {
                if (await _webhookDefinitionManager.IsAvailableAsync(companyId, webhookName))
                {
                    featureGrantedCompanys.Add(companyId);
                }
            }

            return (await WebhookSubscriptionsStore.GetAllSubscriptionsOfCompanysAsync(featureGrantedCompanys.ToArray(),
                    webhookName))
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual List<WebhookSubscription> GetAllSubscriptionsOfCompanysIfFeaturesGranted(int?[] companyIds,
            string webhookName)
        {
            List<int?> featureGrantedCompanys = new List<int?>();
            foreach (int? companyId in companyIds)
            {
                if (_webhookDefinitionManager.IsAvailable(companyId, webhookName))
                {
                    featureGrantedCompanys.Add(companyId);
                }
            }

            return WebhookSubscriptionsStore
                .GetAllSubscriptionsOfCompanys(featureGrantedCompanys.ToArray(), webhookName)
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public virtual async Task<bool> IsSubscribedAsync(int? companyId, string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(companyId, webhookName))
            {
                return false;
            }

            return await WebhookSubscriptionsStore.IsSubscribedAsync(companyId, webhookName);
        }

        public virtual bool IsSubscribed(int? companyId, string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(companyId, webhookName))
            {
                return false;
            }

            return WebhookSubscriptionsStore.IsSubscribed(companyId, webhookName);
        }

        public virtual async Task AddOrUpdateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await CheckIfPermissionsGrantedAsync(webhookSubscription);

                if (webhookSubscription.Id == default)
                {
                    webhookSubscription.Id = _guidGenerator.Create();
                    webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString("N");
                    await WebhookSubscriptionsStore.InsertAsync(webhookSubscription.ToWebhookSubscriptionInfo());
                }
                else
                {
                    WebhookSubscriptionInfo subscription =
                        await WebhookSubscriptionsStore.GetAsync(webhookSubscription.Id);
                    subscription.WebhookUri = webhookSubscription.WebhookUri;
                    subscription.Webhooks = webhookSubscription.Webhooks.ToJsonString();
                    subscription.Headers = webhookSubscription.Headers.ToJsonString();
                    await WebhookSubscriptionsStore.UpdateAsync(subscription);
                }
            });
        }

        public virtual void AddOrUpdateSubscription(WebhookSubscription webhookSubscription)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                CheckIfPermissionsGranted(webhookSubscription);

                if (webhookSubscription.Id == default)
                {
                    webhookSubscription.Id = _guidGenerator.Create();
                    webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString("N");
                    WebhookSubscriptionsStore.Insert(webhookSubscription.ToWebhookSubscriptionInfo());
                }
                else
                {
                    WebhookSubscriptionInfo subscription = WebhookSubscriptionsStore.Get(webhookSubscription.Id);
                    subscription.WebhookUri = webhookSubscription.WebhookUri;
                    subscription.Webhooks = webhookSubscription.Webhooks.ToJsonString();
                    subscription.Headers = webhookSubscription.Headers.ToJsonString();
                    WebhookSubscriptionsStore.Update(subscription);
                }
            });
        }

        public virtual async Task ActivateWebhookSubscriptionAsync(Guid id, bool active)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                WebhookSubscriptionInfo webhookSubscription = await WebhookSubscriptionsStore.GetAsync(id);
                webhookSubscription.IsActive = active;
            });
        }

        public virtual async Task DeleteSubscriptionAsync(Guid id)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await WebhookSubscriptionsStore.DeleteAsync(id);
            });
        }

        public virtual void DeleteSubscription(Guid id)
        {
            UnitOfWorkManager.WithUnitOfWork(() => { WebhookSubscriptionsStore.Delete(id); });
        }

        public virtual async Task AddWebhookAsync(WebhookSubscriptionInfo subscription, string webhookName)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await CheckPermissionsAsync(subscription.CompanyId, webhookName);
                subscription.SubscribeWebhook(webhookName);
            });
        }

        public virtual void AddWebhook(WebhookSubscriptionInfo subscription, string webhookName)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                CheckPermissions(subscription.CompanyId, webhookName);
                subscription.SubscribeWebhook(webhookName);
            });
        }

        #region PermissionCheck

        protected virtual async Task CheckIfPermissionsGrantedAsync(WebhookSubscription webhookSubscription)
        {
            if (webhookSubscription.Webhooks.IsNullOrEmpty())
            {
                return;
            }

            foreach (string webhookDefinition in webhookSubscription.Webhooks)
            {
                await CheckPermissionsAsync(webhookSubscription.CompanyId, webhookDefinition);
            }
        }

        protected virtual async Task CheckPermissionsAsync(int? companyId, string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(companyId, webhookName))
            {
                throw new KontecgAuthorizationException(
                    $"Company \"{companyId}\" must have necessary feature(s) to use webhook \"{webhookName}\"");
            }
        }

        protected virtual void CheckIfPermissionsGranted(WebhookSubscription webhookSubscription)
        {
            if (webhookSubscription.Webhooks.IsNullOrEmpty())
            {
                return;
            }

            foreach (string webhookDefinition in webhookSubscription.Webhooks)
            {
                CheckPermissions(webhookSubscription.CompanyId, webhookDefinition);
            }
        }

        protected virtual void CheckPermissions(int? companyId, string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(companyId, webhookName))
            {
                throw new KontecgAuthorizationException(
                    $"Company \"{companyId}\" must have necessary feature(s) to use webhook \"{webhookName}\"");
            }
        }

        #endregion
    }
}
