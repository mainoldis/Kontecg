using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Webhooks.Extensions;

namespace Kontecg.Webhooks
{
    [Table("webhook_subscriptions", Schema = "job")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class WebhookSubscriptionInfo : CreationAuditedEntity<Guid>, IPassivable
    {
        public WebhookSubscriptionInfo()
        {
            IsActive = true;
        }

        /// <summary>
        ///     Subscribed Company's id .
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        ///     Subscription webhook endpoint
        /// </summary>
        [Required]
        public string WebhookUri { get; set; }

        /// <summary>
        ///     Webhook secret
        /// </summary>
        [Required]
        public string Secret { get; set; }

        /// <summary>
        ///     Subscribed webhook definitions unique names.It contains webhook definitions list as json
        ///     <para>
        ///         Do not change it manually.
        ///         Use <see cref=" WebhookSubscriptionInfoExtensions.GetSubscribedWebhooks" />,
        ///         <see cref=" WebhookSubscriptionInfoExtensions.SubscribeWebhook" />,
        ///         <see cref="WebhookSubscriptionInfoExtensions.UnsubscribeWebhook" /> and
        ///         <see cref="WebhookSubscriptionInfoExtensions.RemoveAllSubscribedWebhooks" /> to change it.
        ///     </para>
        /// </summary>
        public string Webhooks { get; set; }

        /// <summary>
        ///     Gets a set of additional HTTP headers.That headers will be sent with the webhook. It contains webhook header
        ///     dictionary as json
        ///     <para>
        ///         Do not change it manually.
        ///         Use <see cref=" WebhookSubscriptionInfoExtensions.GetWebhookHeaders" />,
        ///         <see cref="WebhookSubscriptionInfoExtensions.AddWebhookHeader" />,
        ///         <see cref="WebhookSubscriptionInfoExtensions.RemoveWebhookHeader" />,
        ///         <see cref="WebhookSubscriptionInfoExtensions.RemoveAllWebhookHeaders" /> to change it.
        ///     </para>
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        ///     Is subscription active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
