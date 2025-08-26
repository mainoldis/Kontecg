using System;
using System.Collections.Generic;
using Kontecg.Application.Services.Dto;
using Kontecg.Domain.Entities;

namespace Kontecg.Webhooks
{
    public class WebhookSubscription : EntityDto<Guid>, IPassivable
    {
        public WebhookSubscription()
        {
            IsActive = true;
            Headers = new Dictionary<string, string>();
            Webhooks = new List<string>();
        }

        /// <summary>
        ///     Company id of the subscribed.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        ///     Subscription webhook endpoint
        /// </summary>
        public string WebhookUri { get; set; }

        /// <summary>
        ///     Webhook secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        ///     Subscribed webhook definitions unique names. <see cref="WebhookDefinition.Name" />
        /// </summary>
        public List<string> Webhooks { get; set; }

        /// <summary>
        ///     Gets a set of additional HTTP headers.That headers will be sent with the webhook.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        ///     Is subscription active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
