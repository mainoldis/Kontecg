using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Application.Clients
{
    [Table("clients", Schema = "seg")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class ClientInfo : FullAuditedAggregateRoot<string>, IExtendableObject, IMustHaveCompany
    {
        /// <summary>
        /// Maximum length of <see cref="IpAddress"/> property.
        /// </summary>
        public static int MaxClientIpAddressLength = 64;

        /// <summary>
        /// Maximum length of <see cref="Name"/> property.
        /// </summary>
        public static int MaxClientNameLength = 128;

        /// <summary>
        /// Maximum length of <see cref="Info"/> property.
        /// </summary>
        public static int MaxClientInfoLength = 2048;

        /// <summary>
        ///     IP address of the client.
        /// </summary>
        [Required]
        public virtual string IpAddress { get; set; }

        /// <summary>
        ///     Name (generally computer name) of the client.
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Client information
        /// </summary>
        [Required]
        public virtual string Info { get; set; }

        /// <summary>
        ///     Base client version (KernelModule)
        /// </summary>
        public virtual string Version { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public virtual string ExtensionData { get; set; }

        public ClientInfo() { }

        public ClientInfo(int companyId, string id, string ipAddress, string computerName, string info, string version)
        {
            Check.NotNullOrEmpty(id, nameof(id));

            Id = id;
            CompanyId = companyId;
            IpAddress = ipAddress;
            Name = computerName;
            Info = info;
            Version = version;
        }

        public override string ToString()
        {
            var v = Version != null
                ? "version: " + Version
                : "succeed";
            return
                $"CLIENT: {Id} is executed on {Name} from {IpAddress} IP address with {v}.";
        }
    }
}
