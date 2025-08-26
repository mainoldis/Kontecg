using System;
using System.Collections.Generic;
using Kontecg.Json;
using Kontecg.Timing;

namespace Kontecg.RealTime
{
    /// <summary>
    ///     Implements <see cref="IOnlineClient" />.
    /// </summary>
    [Serializable]
    public class OnlineClient : IOnlineClient
    {
        private Dictionary<string, object> _properties;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnlineClient" /> class.
        /// </summary>
        public OnlineClient()
        {
            ConnectTime = Clock.Now;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnlineClient" /> class.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public OnlineClient(string connectionId, string ipAddress, int? companyId, long? userId)
            : this()
        {
            ConnectionId = connectionId;
            IpAddress = ipAddress;
            CompanyId = companyId;
            UserId = userId;

            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        ///     Unique connection Id for this client.
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        ///     IP address of this client.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///     Company Id.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        ///     User Id.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        ///     Connection establishment time for this client.
        /// </summary>
        public DateTime ConnectTime { get; set; }

        /// <summary>
        ///     Shortcut to set/get <see cref="Properties" />.
        /// </summary>
        public object this[string key]
        {
            get => Properties[key];
            set => Properties[key] = value;
        }

        /// <summary>
        ///     Can be used to add custom properties for this client.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get => _properties;
            set => _properties = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString()
        {
            return this.ToJsonString();
        }
    }
}
