using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Application.Clients;

namespace Kontecg.Application.Features
{
    /// <summary>
    /// Feature setting for an <see cref="Clients.ClientInfo"/>.
    /// </summary>
    public class ClientFeatureSetting : FeatureSetting
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        [ForeignKey("ClientId")]
        public virtual ClientInfo Client { get; set; }

        /// <summary>
        /// Gets or sets the client Id.
        /// </summary>
        /// <value>
        /// The client Id.
        /// </value>
        public virtual string ClientId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientFeatureSetting"/> class.
        /// </summary>
        public ClientFeatureSetting()
        {
        }

        /// <inheritdoc />
        public ClientFeatureSetting(string clientId, string name, string value)
            :base(name, value)
        {
            ClientId = clientId;
        }
    }
}
