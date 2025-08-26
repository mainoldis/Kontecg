using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Events.Bus.Entities;

namespace Kontecg.EntityHistory
{
    [Table("entity_changes", Schema = "log")]
    public class EntityChange : Entity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of <see cref="EntityId" /> property.
        ///     Value: 48.
        /// </summary>
        public const int EntityIdLength = 48;

        /// <summary>
        ///     Maximum length of <see cref="EntityTypeFullName" /> property.
        ///     Value: 192.
        /// </summary>
        public const int EntityTypeFullNameLength = 192;

        /// <summary>
        ///     ChangeTime.
        /// </summary>
        public virtual DateTime ChangeTime { get; set; }

        /// <summary>
        ///     ChangeType.
        /// </summary>
        public virtual EntityChangeType ChangeType { get; set; }

        /// <summary>
        ///     Gets/sets change set id, used to group entity changes.
        /// </summary>
        public virtual long EntityChangeSetId { get; set; }

        /// <summary>
        ///     Gets/sets primary key of the entity.
        /// </summary>
        [StringLength(EntityIdLength)]
        public virtual string EntityId { get; set; }

        /// <summary>
        ///     FullName of the entity type.
        /// </summary>
        [StringLength(EntityTypeFullNameLength)]
        public virtual string EntityTypeFullName { get; set; }

        /// <summary>
        ///     PropertyChanges.
        /// </summary>
        public virtual ICollection<EntityPropertyChange> PropertyChanges { get; set; }

        #region Not mapped

        [NotMapped] public virtual object EntityEntry { get; set; }

        #endregion

        /// <summary>
        ///     CompanyId.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
