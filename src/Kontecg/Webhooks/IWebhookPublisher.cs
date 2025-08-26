using System.Threading.Tasks;
using Kontecg.Runtime.Session;

namespace Kontecg.Webhooks
{
    public interface IWebhookPublisher
    {
        /// <summary>
        ///     Sends webhooks to current company subscriptions (<see cref="IKontecgSession.CompanyId" />). with given data,
        ///     (Checks
        ///     permissions)
        /// </summary>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <param name="data">data to send</param>
        /// <param name="sendExactSameData">
        ///     True: It sends the exact same data as the parameter to clients.
        ///     <para>
        ///         False: It sends data in <see cref="WebhookPayload" />. It is recommended way.
        ///     </para>
        /// </param>
        Task PublishAsync(string webhookName, object data, bool sendExactSameData = false);

        /// <summary>
        ///     Sends webhooks to current company subscriptions (<see cref="IKontecgSession.CompanyId" />). with given data,
        ///     (Checks
        ///     permissions)
        /// </summary>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <param name="data">data to send</param>
        /// <param name="sendExactSameData">
        ///     True: It sends the exact same data as the parameter to clients.
        ///     <para>
        ///         False: It sends data in <see cref="WebhookPayload" />. It is recommended way.
        ///     </para>
        /// </param>
        void Publish(string webhookName, object data, bool sendExactSameData = false);

        /// <summary>
        ///     Sends webhooks to given company's subscriptions
        /// </summary>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <param name="data">data to send</param>
        /// <param name="companyId">
        ///     Target company id
        /// </param>
        /// <param name="sendExactSameData">
        ///     True: It sends the exact same data as the parameter to clients.
        ///     <para>
        ///         False: It sends data in <see cref="WebhookPayload" />. It is recommended way.
        ///     </para>
        /// </param>
        Task PublishAsync(string webhookName, object data, int? companyId, bool sendExactSameData = false);

        /// <summary>
        ///     Sends webhooks to given company's subscriptions
        /// </summary>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <param name="data">data to send</param>
        /// <param name="companyId">
        ///     Target company id
        /// </param>
        /// <param name="sendExactSameData">
        ///     True: It sends the exact same data as the parameter to clients.
        ///     <para>
        ///         False: It sends data in <see cref="WebhookPayload" />. It is recommended way.
        ///     </para>
        /// </param>
        void Publish(string webhookName, object data, int? companyId, bool sendExactSameData = false);

        /// <summary>
        ///     Sends webhooks to given company's subscriptions
        /// </summary>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <param name="data">data to send</param>
        /// <param name="companyIds">
        ///     Target company id(s)
        /// </param>
        /// <param name="sendExactSameData">
        ///     True: It sends the exact same data as the parameter to clients.
        ///     <para>
        ///         False: It sends data in <see cref="WebhookPayload" />. It is recommended way.
        ///     </para>
        /// </param>
        Task PublishAsync(int?[] companyIds, string webhookName, object data, bool sendExactSameData = false);

        /// <summary>
        ///     Sends webhooks to given company's subscriptions
        /// </summary>
        /// <param name="webhookName">
        ///     <see cref="WebhookDefinition.Name" />
        /// </param>
        /// <param name="data">data to send</param>
        /// <param name="companyIds">
        ///     Target company id(s)
        /// </param>
        /// <param name="sendExactSameData">
        ///     True: It sends the exact same data as the parameter to clients.
        ///     <para>
        ///         False: It sends data in <see cref="WebhookPayload" />. It is recommended way.
        ///     </para>
        /// </param>
        void Publish(int?[] companyIds, string webhookName, object data, bool sendExactSameData = false);
    }
}
