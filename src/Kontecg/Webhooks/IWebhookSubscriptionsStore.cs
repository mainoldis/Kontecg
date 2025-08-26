using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Webhooks
{
    /// <summary>
    ///     This interface should be implemented by vendors to make webhooks working.
    /// </summary>
    public interface IWebhookSubscriptionsStore
    {
        /// <summary>
        ///     returns subscription
        /// </summary>
        /// <param name="id">webhook subscription id</param>
        /// <returns></returns>
        Task<WebhookSubscriptionInfo> GetAsync(Guid id);

        /// <summary>
        ///     returns subscription
        /// </summary>
        /// <param name="id">webhook subscription id</param>
        /// <returns></returns>
        WebhookSubscriptionInfo Get(Guid id);

        /// <summary>
        ///     Saves webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webhookSubscription">webhook subscription information</param>
        Task InsertAsync(WebhookSubscriptionInfo webhookSubscription);

        /// <summary>
        ///     Saves webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webhookSubscription">webhook subscription information</param>
        void Insert(WebhookSubscriptionInfo webhookSubscription);

        /// <summary>
        ///     Updates webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webhookSubscription">webhook subscription information</param>
        Task UpdateAsync(WebhookSubscriptionInfo webhookSubscription);

        /// <summary>
        ///     Updates webhook subscription to a persistent store.
        /// </summary>
        /// <param name="webhookSubscription">webhook subscription information</param>
        void Update(WebhookSubscriptionInfo webhookSubscription);

        /// <summary>
        ///     Deletes subscription if exists
        /// </summary>
        /// <param name="id"><see cref="WebhookSubscriptionInfo" /> primary key</param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        ///     Deletes subscription if exists
        /// </summary>
        /// <param name="id"><see cref="WebhookSubscriptionInfo" /> primary key</param>
        /// <returns></returns>
        void Delete(Guid id);

        /// <summary>
        ///     Returns all subscriptions of given company including deactivated
        /// </summary>
        /// <param name="companyId">
        ///     Target company id.
        /// </param>
        Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? companyId);

        /// <summary>
        ///     Returns all subscriptions of given company including deactivated
        /// </summary>
        /// <param name="companyId">
        ///     Target company id.
        /// </param>
        List<WebhookSubscriptionInfo> GetAllSubscriptions(int? companyId);

        /// <summary>
        ///     Returns webhook subscriptions which subscribe to given webhook on company(s)
        /// </summary>
        /// <param name="companyId">
        ///     Target company id.
        /// </param>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <returns></returns>
        Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? companyId, string webhookName);

        /// <summary>
        ///     Returns webhook subscriptions which subscribe to given webhook on company(s)
        /// </summary>
        /// <param name="companyId">
        ///     Target company id.
        /// </param>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <returns></returns>
        List<WebhookSubscriptionInfo> GetAllSubscriptions(int? companyId, string webhookName);

        /// <summary>
        ///     Returns all subscriptions of given company including deactivated
        /// </summary>
        /// <param name="companyIds">
        ///     Target company id(s).
        /// </param>
        Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfCompanysAsync(int?[] companyIds);

        /// <summary>
        ///     Returns all subscriptions of given company including deactivated
        /// </summary>
        /// <param name="companyIds">
        ///     Target company id(s).
        /// </param>
        List<WebhookSubscriptionInfo> GetAllSubscriptionsOfCompanys(int?[] companyIds);

        /// <summary>
        ///     Returns webhook subscriptions which subscribe to given webhook on company(s)
        /// </summary>
        /// <param name="companyIds">
        ///     Target company id(s).
        /// </param>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <returns></returns>
        Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfCompanysAsync(int?[] companyIds, string webhookName);

        /// <summary>
        ///     Returns webhook subscriptions which subscribe to given webhook on company(s)
        /// </summary>
        /// <param name="companyIds">
        ///     Target company id(s).
        /// </param>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <returns></returns>
        List<WebhookSubscriptionInfo> GetAllSubscriptionsOfCompanys(int?[] companyIds, string webhookName);


        /// <summary>
        ///     Checks if company subscribed for a webhook
        /// </summary>
        /// <param name="companyId">
        ///     Target company id(s).
        /// </param>
        /// <param name="webhookName">Name of the webhook</param>
        Task<bool> IsSubscribedAsync(int? companyId, string webhookName);

        /// <summary>
        ///     Checks if company subscribed for a webhook
        /// </summary>
        /// <param name="companyId">
        ///     Target company id(s).
        /// </param>
        /// <param name="webhookName">Name of the webhook</param>
        bool IsSubscribed(int? companyId, string webhookName);
    }
}
