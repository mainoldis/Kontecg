using System.Collections.Generic;

namespace Kontecg.Data
{
    public class ActiveTransactionProviderArgs : Dictionary<string, object>
    {
        public static ActiveTransactionProviderArgs Empty { get; } = new();
    }
}
