using System.Collections.Generic;
using Kontecg.Domain.Uow;

namespace Kontecg.MultiCompany
{
    public class DbPerCompanyConnectionStringResolveArgs : ConnectionStringResolveArgs
    {
        public DbPerCompanyConnectionStringResolveArgs(int? companyId, MultiCompanySides? multiCompanySide = null)
            : base(multiCompanySide)
        {
            CompanyId = companyId;
        }

        public DbPerCompanyConnectionStringResolveArgs(int? companyId, ConnectionStringResolveArgs baseArgs)
        {
            CompanyId = companyId;
            MultiCompanySide = baseArgs.MultiCompanySide;

            foreach (KeyValuePair<string, object> kvPair in baseArgs)
            {
                Add(kvPair.Key, kvPair.Value);
            }
        }

        public int? CompanyId { get; set; }
    }
}
