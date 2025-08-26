using System.Collections.Generic;

namespace Kontecg.Auditing
{
    public class NullClientInfoProvider : IClientInfoProvider
    {
        public static NullClientInfoProvider Instance { get; } = new();

        public string ClientId => null;
        public string ClientInfo => null;
        public string ClientIpAddress => null;
        public string ComputerName => null;
        public string Version => null;
        public Dictionary<string, object> Properties => new();
    }
}
