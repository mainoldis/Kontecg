using System;
using Kontecg.Collections;
using Kontecg.Json;
using Newtonsoft.Json;

namespace Kontecg.Webhooks
{
    public interface IWebhooksConfiguration
    {
        /// <summary>
        ///     HttpClient timeout. Sender will wait <c>TimeoutDuration</c> second before throw timeout exception
        /// </summary>
        TimeSpan TimeoutDuration { get; set; }

        /// <summary>
        ///     How many times <see cref="IWebhookPublisher" /> will try resend webhook until gets HttpStatusCode.OK
        /// </summary>
        int SendAttemptCount { get; set; }

        /// <summary>
        ///     Json serializer settings for converting webhook data to json, If this is null default settings will be used.
        ///     <see cref="JsonExtensions.ToJsonString(object,bool,bool)" />
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        ///     Webhook providers.
        /// </summary>
        ITypeList<WebhookDefinitionProvider> Providers { get; }

        /// <summary>
        ///     If you enable that, subscriptions will be automatically disabled if they fails
        ///     <see cref="ConsecutiveFailCountBeforeDeactivateSubscription" /> times consecutively.
        ///     Companys should activate it back manually.
        /// </summary>
        bool IsAutomaticSubscriptionDeactivationEnabled { get; set; }

        /// <summary>
        ///     If you enable <see cref="IsAutomaticSubscriptionDeactivationEnabled" />, subscriptions will be automatically
        ///     disabled if they fails <see cref="ConsecutiveFailCountBeforeDeactivateSubscription" /> times consecutively.
        /// </summary>
        int ConsecutiveFailCountBeforeDeactivateSubscription { get; set; }
    }
}
