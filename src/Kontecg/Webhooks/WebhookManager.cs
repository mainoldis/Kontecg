using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Json;

namespace Kontecg.Webhooks
{
    public class WebhookManager : DomainService, IWebhookManager
    {
        private const string SignatureHeaderKey = "sha256";
        private const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";
        private const string SignatureHeaderName = "kontecg-webhook-signature";

        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly IWebhookSendAttemptStore _webhookSendAttemptStore;

        public WebhookManager(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookSendAttemptStore webhookSendAttemptStore)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _webhookSendAttemptStore = webhookSendAttemptStore;
        }

        public virtual async Task<WebhookPayload> GetWebhookPayloadAsync(WebhookSenderArgs webhookSenderArgs)
        {
            dynamic data = _webhooksConfiguration.JsonSerializerSettings != null
                ? webhookSenderArgs.Data.FromJsonString<dynamic>(_webhooksConfiguration.JsonSerializerSettings)
                : webhookSenderArgs.Data.FromJsonString<dynamic>();

            int attemptNumber = await _webhookSendAttemptStore.GetSendAttemptCountAsync(
                webhookSenderArgs.CompanyId,
                webhookSenderArgs.WebhookEventId,
                webhookSenderArgs.WebhookSubscriptionId);

            return new WebhookPayload(
                webhookSenderArgs.WebhookEventId.ToString(),
                webhookSenderArgs.WebhookName,
                attemptNumber)
            {
                Data = data
            };
        }

        public virtual WebhookPayload GetWebhookPayload(WebhookSenderArgs webhookSenderArgs)
        {
            dynamic data = _webhooksConfiguration.JsonSerializerSettings != null
                ? webhookSenderArgs.Data.FromJsonString<dynamic>(_webhooksConfiguration.JsonSerializerSettings)
                : webhookSenderArgs.Data.FromJsonString<dynamic>();

            int attemptNumber = _webhookSendAttemptStore.GetSendAttemptCount(
                webhookSenderArgs.CompanyId,
                webhookSenderArgs.WebhookEventId,
                webhookSenderArgs.WebhookSubscriptionId) + 1;

            return new WebhookPayload(
                webhookSenderArgs.WebhookEventId.ToString(),
                webhookSenderArgs.WebhookName,
                attemptNumber)
            {
                Data = data
            };
        }

        public virtual void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(serializedBody))
            {
                throw new ArgumentNullException(nameof(serializedBody));
            }

            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

            using HMACSHA256 hasher = new HMACSHA256(secretBytes);
            request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");

            byte[] data = Encoding.UTF8.GetBytes(serializedBody);
            byte[] sha256 = hasher.ComputeHash(data);

            string headerValue = string.Format(CultureInfo.InvariantCulture, SignatureHeaderValueTemplate,
                BitConverter.ToString(sha256));

            request.Headers.Add(SignatureHeaderName, headerValue);
        }

        public virtual string GetSerializedBody(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.SendExactSameData)
            {
                return webhookSenderArgs.Data;
            }

            WebhookPayload payload = GetWebhookPayload(webhookSenderArgs);

            string serializedBody = _webhooksConfiguration.JsonSerializerSettings != null
                ? payload.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                : payload.ToJsonString();

            return serializedBody;
        }

        public virtual async Task<string> GetSerializedBodyAsync(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.SendExactSameData)
            {
                return webhookSenderArgs.Data;
            }

            WebhookPayload payload = await GetWebhookPayloadAsync(webhookSenderArgs);

            string serializedBody = _webhooksConfiguration.JsonSerializerSettings != null
                ? payload.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                : payload.ToJsonString();

            return serializedBody;
        }

        public virtual async Task<Guid> InsertAndGetIdWebhookSendAttemptAsync(WebhookSenderArgs webhookSenderArgs)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            WebhookSendAttempt workItem = new WebhookSendAttempt
            {
                WebhookEventId = webhookSenderArgs.WebhookEventId,
                WebhookSubscriptionId = webhookSenderArgs.WebhookSubscriptionId,
                CompanyId = webhookSenderArgs.CompanyId
            };

            await _webhookSendAttemptStore.InsertAsync(workItem);
            await CurrentUnitOfWork.SaveChangesAsync();

            await uow.CompleteAsync();

            return workItem.Id;
        }

        public virtual async Task StoreResponseOnWebhookSendAttemptAsync(Guid webhookSendAttemptId, int? companyId,
            HttpStatusCode? statusCode, string content)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            WebhookSendAttempt webhookSendAttempt =
                await _webhookSendAttemptStore.GetAsync(companyId, webhookSendAttemptId);

            webhookSendAttempt.ResponseStatusCode = statusCode;
            webhookSendAttempt.Response = content;

            await _webhookSendAttemptStore.UpdateAsync(webhookSendAttempt);

            await uow.CompleteAsync();
        }
    }
}
