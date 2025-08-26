using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Webhooks
{
    /// <summary>
    ///     Store created web hooks. To see who get that webhook check with <see cref="WebhookSendAttempt.WebhookEventId" />
    ///     and you can get <see cref="WebhookSendAttempt.WebhookSubscriptionId" />
    /// </summary>
    [Table("webhook_events", Schema = "job")]
    public class WebhookEvent : Entity<Guid>, IMayHaveCompany, IHasCreationTime, IHasDeletionTime
    {
        /// <summary>
        ///     Webhook unique name <see cref="WebhookDefinition.Name" />
        /// </summary>
        [Required]
        public string WebhookName { get; set; }

        /// <summary>
        ///     Webhook data as JSON string.
        /// </summary>
        public string Data { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public int? CompanyId { get; set; }
    }
}
