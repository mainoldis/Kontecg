using System.Collections.Generic;
using Kontecg.MultiCompany;

namespace Kontecg.Domain.Uow
{
    public class ConnectionStringResolveArgs : Dictionary<string, object>
    {
        public MultiCompanySides? MultiCompanySide { get; set; }

        public ConnectionStringResolveArgs(MultiCompanySides? multiCompanySide = null)
        {
            MultiCompanySide = multiCompanySide;
        }
    }
}
