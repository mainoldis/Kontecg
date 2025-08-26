using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Kontecg.Domain.Services;

namespace Kontecg.Webhooks
{
    public class DefaultWebhookSender : DomainService, IWebhookSender
    {
        private const string FailedRequestDefaultContent = "Webhook Send Request Failed";
        private readonly IWebhookManager _webhookManager;
        private readonly IWebhooksConfiguration _webhooksConfiguration;

        public DefaultWebhookSender(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookManager webhookManager)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _webhookManager = webhookManager;
        }

        public async Task<Guid> SendWebhookAsync(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.WebhookEventId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookEventId));
            }

            if (webhookSenderArgs.WebhookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookSubscriptionId));
            }

            Guid webhookSendAttemptId = await _webhookManager.InsertAndGetIdWebhookSendAttemptAsync(webhookSenderArgs);

            HttpRequestMessage request = CreateWebhookRequestMessage(webhookSenderArgs);

            string serializedBody = await _webhookManager.GetSerializedBodyAsync(webhookSenderArgs);

            _webhookManager.SignWebhookRequest(request, serializedBody, webhookSenderArgs.Secret);

            AddAdditionalHeaders(request, webhookSenderArgs);

            bool isSucceed = false;
            HttpStatusCode? statusCode = null;
            string content = FailedRequestDefaultContent;

            try
            {
                (bool isSucceed, HttpStatusCode statusCode, string content) response =
                    await SendHttpRequestAsync(request);
                isSucceed = response.isSucceed;
                statusCode = response.statusCode;
                content = response.content;
            }
            catch (TaskCanceledException)
            {
                statusCode = HttpStatusCode.RequestTimeout;
                content = "Request Timeout";
            }
            catch (HttpRequestException e)
            {
                content = e.Message;
            }
            catch (Exception e)
            {
                Logger.Error("An error occured while sending a webhook request", e);
            }
            finally
            {
                await _webhookManager.StoreResponseOnWebhookSendAttemptAsync(webhookSendAttemptId,
                    webhookSenderArgs.CompanyId, statusCode, content);
            }

            if (!isSucceed)
            {
                throw new Exception($"Webhook sending attempt failed. WebhookSendAttempt id: {webhookSendAttemptId}");
            }

            return webhookSendAttemptId;
        }

        /// <summary>
        ///     You can override this to change request message
        /// </summary>
        /// <returns></returns>
        protected virtual HttpRequestMessage CreateWebhookRequestMessage(WebhookSenderArgs webhookSenderArgs)
        {
            return new HttpRequestMessage(HttpMethod.Post, webhookSenderArgs.WebhookUri);
        }

        protected virtual void AddAdditionalHeaders(HttpRequestMessage request, WebhookSenderArgs webhookSenderArgs)
        {
            foreach (KeyValuePair<string, string> header in webhookSenderArgs.Headers)
            {
                if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }

                if (request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }

                throw new Exception(
                    $"Invalid Header. SubscriptionId:{webhookSenderArgs.WebhookSubscriptionId},Header: {header.Key}:{header.Value}");
            }
        }

        protected virtual async Task<(bool isSucceed, HttpStatusCode statusCode, string content)> SendHttpRequestAsync(
            HttpRequestMessage request)
        {
            using HttpClient client = new HttpClient
            {
                Timeout = _webhooksConfiguration.TimeoutDuration
            };
            HttpResponseMessage response = await client.SendAsync(request);

            bool isSucceed = response.IsSuccessStatusCode;
            HttpStatusCode statusCode = response.StatusCode;
            string content = await response.Content.ReadAsStringAsync();

            return (isSucceed, statusCode, content);
        }
    }
}
