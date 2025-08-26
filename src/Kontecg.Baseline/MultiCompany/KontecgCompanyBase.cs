using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Runtime.Security;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Base class for companies.
    /// </summary>
    [Table("companies", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public abstract class KontecgCompanyBase : FullAuditedEntity<int>, IPassivable
    {
        /// <summary>
        ///     Max length of the <see cref="CompanyName" /> property.
        /// </summary>
        public const int MaxCompanyNameLength = 64;

        /// <summary>
        ///     Max length of the <see cref="ConnectionString" /> property.
        /// </summary>
        public const int MaxConnectionStringLength = 1024;

        /// <summary>
        ///     "ECG".
        /// </summary>
        public const string DefaultCompanyName = "ECG";

        /// <summary>
        ///     "ECG".
        /// </summary>
        public const string DefaultName = "Empresa Cmdtte. Ernesto Che Guevara";

        /// <summary>
        ///     "^[a-zA-Z][a-zA-Z0-9_-]{1,}$".
        /// </summary>
        public const string CompanyNameRegex = "^[a-zA-Z][a-zA-Z0-9_-]{1,}$";

        /// <summary>
        ///     Max length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxNameLength = 500;

        /// <summary>
        ///     Company name. This property is the UNIQUE name of this Company.
        ///     It can be used as subdomain name in a web application.
        /// </summary>
        [Required]
        [StringLength(MaxCompanyNameLength)]
        public virtual string CompanyName { get; set; }

        /// <summary>
        ///     Display name of the Company.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     ENCRYPTED connection string of the company database.
        ///     Can be null if this company is stored in host database.
        ///     Use <see cref="SimpleStringCipher" /> to encrypt/decrypt this.
        /// </summary>
        [StringLength(MaxConnectionStringLength)]
        public virtual string ConnectionString { get; set; }

        /// <summary>
        ///     Is this company active?
        ///     If as company is not active, no user of this company can use the application.
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}
