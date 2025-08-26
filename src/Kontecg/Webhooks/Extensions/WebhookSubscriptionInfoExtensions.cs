using System;
using System.Collections.Generic;
using Kontecg.Extensions;
using Kontecg.Json;

namespace Kontecg.Webhooks.Extensions
{
    public static class WebhookSubscriptionInfoExtensions
    {
        /// <summary>
        ///     Return List of subscribed webhooks definitions <see cref="WebhookSubscriptionInfo.Webhooks" />
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSubscribedWebhooks(this WebhookSubscriptionInfo webhookSubscription)
        {
            if (webhookSubscription.Webhooks.IsNullOrWhiteSpace())
            {
                return new List<string>();
            }

            return webhookSubscription.Webhooks.FromJsonString<List<string>>();
        }

        /// <summary>
        ///     Adds webhook subscription to <see cref="WebhookSubscriptionInfo.Webhooks" /> if not exists
        /// </summary>
        /// <param name="webhookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void SubscribeWebhook(this WebhookSubscriptionInfo webhookSubscription, string name)
        {
            name = name.Trim();
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            List<string> webhookDefinitions = webhookSubscription.GetSubscribedWebhooks();
            if (webhookDefinitions.Contains(name))
            {
                return;
            }

            webhookDefinitions.Add(name);
            webhookSubscription.Webhooks = webhookDefinitions.ToJsonString();
        }

        /// <summary>
        ///     Removes webhook subscription from <see cref="WebhookSubscriptionInfo.Webhooks" /> if exists
        /// </summary>
        /// <param name="webhookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void UnsubscribeWebhook(this WebhookSubscriptionInfo webhookSubscription, string name)
        {
            name = name.Trim();
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            List<string> webhookDefinitions = webhookSubscription.GetSubscribedWebhooks();
            if (!webhookDefinitions.Contains(name))
            {
                return;
            }

            webhookDefinitions.Remove(name);
            webhookSubscription.Webhooks = webhookDefinitions.ToJsonString();
        }

        /// <summary>
        ///     Clears all <see cref="WebhookSubscriptionInfo.Webhooks" />
        /// </summary>
        /// <param name="webhookSubscription"></param>
        public static void RemoveAllSubscribedWebhooks(this WebhookSubscriptionInfo webhookSubscription)
        {
            webhookSubscription.Webhooks = null;
        }

        /// <summary>
        ///     if subscribed to given webhook
        /// </summary>
        /// <returns></returns>
        public static bool IsSubscribed(this WebhookSubscriptionInfo webhookSubscription, string webhookName)
        {
            if (webhookSubscription.Webhooks.IsNullOrWhiteSpace())
            {
                return false;
            }

            return webhookSubscription.GetSubscribedWebhooks().Contains(webhookName);
        }

        /// <summary>
        ///     Returns additional webhook headers <see cref="WebhookSubscriptionInfo.Headers" />
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetWebhookHeaders(this WebhookSubscriptionInfo webhookSubscription)
        {
            return webhookSubscription.Headers.IsNullOrWhiteSpace()
                ? new Dictionary<string, string>()
                : webhookSubscription.Headers.FromJsonString<Dictionary<string, string>>();
        }

        /// <summary>
        ///     Adds webhook subscription to <see cref="WebhookSubscriptionInfo.Webhooks" /> if not exists
        /// </summary>
        public static void AddWebhookHeader(this WebhookSubscriptionInfo webhookSubscription, string key, string value)
        {
            if (key.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(key), $"{nameof(key)} can not be null, empty or whitespace!");
            }

            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(value),
                    $"{nameof(value)} can not be null, empty or whitespace!");
            }

            IDictionary<string, string> headers = webhookSubscription.GetWebhookHeaders();
            headers[key] = value;

            webhookSubscription.Headers = headers.ToJsonString();
        }

        /// <summary>
        ///     Adds webhook subscription to <see cref="WebhookSubscriptionInfo.Webhooks" /> if not exists
        /// </summary>
        /// <param name="webhookSubscription"></param>
        /// <param name="header">Key of header</param>
        public static void RemoveWebhookHeader(this WebhookSubscriptionInfo webhookSubscription, string header)
        {
            if (header.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(header),
                    $"{nameof(header)} can not be null, empty or whitespace!");
            }

            IDictionary<string, string> headers = webhookSubscription.GetWebhookHeaders();

            if (!headers.ContainsKey(header))
            {
                return;
            }

            headers.Remove(header);

            webhookSubscription.Headers = headers.ToJsonString();
        }

        /// <summary>
        ///     Clears all <see cref="WebhookSubscriptionInfo.Webhooks" />
        /// </summary>
        /// <param name="webhookSubscription"></param>
        public static void RemoveAllWebhookHeaders(this WebhookSubscriptionInfo webhookSubscription)
        {
            webhookSubscription.Headers = null;
        }

        public static WebhookSubscriptionInfo ToWebhookSubscriptionInfo(this WebhookSubscription webhookSubscription)
        {
            return new WebhookSubscriptionInfo
            {
                Id = webhookSubscription.Id,
                CompanyId = webhookSubscription.CompanyId,
                IsActive = webhookSubscription.IsActive,
                Secret = webhookSubscription.Secret,
                WebhookUri = webhookSubscription.WebhookUri,
                Webhooks = webhookSubscription.Webhooks.ToJsonString(),
                Headers = webhookSubscription.Headers.ToJsonString()
            };
        }
    }
}
