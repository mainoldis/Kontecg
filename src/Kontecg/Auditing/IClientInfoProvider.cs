using System.Collections.Generic;

namespace Kontecg.Auditing
{
    public interface IClientInfoProvider
    {
        string ClientId { get; }

        string ClientInfo { get; }

        string ClientIpAddress { get; }

        string ComputerName { get; }

        string Version { get; }

        Dictionary<string, object> Properties { get; }
    }
}
