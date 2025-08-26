using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Kontecg.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookSubscriptionsStore"/> using repositories.
    /// </summary>
    public class WebhookSubscriptionsStore : IWebhookSubscriptionsStore, ITransientDependency
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<WebhookSubscriptionInfo, Guid> _webhookSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebhookSubscriptionsStore(
            IRepository<WebhookSubscriptionInfo, Guid> webhookSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookSubscriptionRepository = webhookSubscriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual async Task<WebhookSubscriptionInfo> GetAsync(Guid id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _webhookSubscriptionRepository.GetAsync(id));
        }

        public virtual WebhookSubscriptionInfo Get(Guid id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Get(id));
        }

        public virtual async Task InsertAsync(WebhookSubscriptionInfo webhookInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _webhookSubscriptionRepository.InsertAsync(webhookInfo);
            });
        }

        public virtual void Insert(WebhookSubscriptionInfo webhookInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Insert(webhookInfo));
        }

        public virtual async Task UpdateAsync(WebhookSubscriptionInfo webhookSubscription)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _webhookSubscriptionRepository.UpdateAsync(webhookSubscription);
            });
        }

        public virtual void Update(WebhookSubscriptionInfo webhookSubscription)
        {
            _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Update(webhookSubscription));
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _webhookSubscriptionRepository.DeleteAsync(id);
            });
        }

        public virtual void Delete(Guid id)
        {
            _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Delete(id));
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? companyId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    subscriptionInfo.CompanyId == companyId);
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptions(int? companyId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    subscriptionInfo.CompanyId == companyId);
            });
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(
            int? companyId,
            string webhookName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    subscriptionInfo.CompanyId == companyId &&
                    subscriptionInfo.IsActive &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptions(int? companyId, string webhookName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    subscriptionInfo.CompanyId == companyId &&
                    subscriptionInfo.IsActive &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfCompanysAsync(int?[] companyIds)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    companyIds.Contains(subscriptionInfo.CompanyId)
                );
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptionsOfCompanys(int?[] companyIds)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    companyIds.Contains(subscriptionInfo.CompanyId)
                );
            });
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfCompanysAsync(
            int?[] companyIds,
            string webhookName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    subscriptionInfo.IsActive &&
                    companyIds.Contains(subscriptionInfo.CompanyId) &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptionsOfCompanys(int?[] companyIds, string webhookName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    subscriptionInfo.IsActive &&
                    companyIds.Contains(subscriptionInfo.CompanyId) &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual async Task<bool> IsSubscribedAsync(int? companyId, string webhookName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await AsyncQueryableExecuter.AnyAsync((await _webhookSubscriptionRepository.GetAllAsync())
                    .Where(subscriptionInfo =>
                        subscriptionInfo.CompanyId == companyId &&
                        subscriptionInfo.IsActive &&
                        subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                    ));
            });
        }

        public virtual bool IsSubscribed(int? companyId, string webhookName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAll()
                    .Any(subscriptionInfo =>
                        subscriptionInfo.CompanyId == companyId &&
                        subscriptionInfo.IsActive &&
                        subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                    );
            });
        }
    }
}
