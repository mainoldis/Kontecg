using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.EntityHistory
{
    [Table("entity_change_sets", Schema = "log")]
    public class EntityChangeSet : Entity<long>, IHasCreationTime, IMayHaveCompany, IExtendableObject
    {
        /// <summary>
        ///     Maximum length of <see cref="ClientInfo" /> property.
        /// </summary>
        public const int MaxClientInfoLength = 512;

        /// <summary>
        ///     Maximum length of <see cref="ClientIpAddress" /> property.
        /// </summary>
        public const int MaxClientIpAddressLength = 64;

        /// <summary>
        ///     Maximum length of <see cref="ClientName" /> property.
        /// </summary>
        public const int MaxClientNameLength = 128;

        /// <summary>
        ///     Maximum length of <see cref="Reason" /> property.
        /// </summary>
        public const int MaxReasonLength = 256;

        public EntityChangeSet()
        {
            EntityChanges = new List<EntityChange>();
        }

        /// <summary>
        ///     Browser information if this entity is changed in a web request.
        /// </summary>
        [StringLength(MaxClientInfoLength)]
        public virtual string ClientInfo { get; set; }

        /// <summary>
        ///     IP address of the client.
        /// </summary>
        [StringLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        ///     Name (generally computer name) of the client.
        /// </summary>
        [StringLength(MaxClientNameLength)]
        public virtual string ClientName { get; set; }

        /// <summary>
        ///     Reason for this change set.
        /// </summary>
        [StringLength(MaxReasonLength)]
        public virtual string Reason { get; set; }

        /// <summary>
        ///     UserId.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        ///     Entity changes grouped in this change set.
        /// </summary>
        public virtual IList<EntityChange> EntityChanges { get; set; }

        /// <summary>
        ///     ImpersonatorCompanyId.
        /// </summary>
        public virtual int? ImpersonatorCompanyId { get; set; }

        /// <summary>
        ///     ImpersonatorUserId.
        /// </summary>
        public virtual long? ImpersonatorUserId { get; set; }

        /// <summary>
        ///     A JSON formatted string to extend the containing object.
        /// </summary>
        public virtual string ExtensionData { get; set; }

        /// <summary>
        ///     Creation time of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        ///     CompanyId.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
