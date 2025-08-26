using Kontecg.Collections;
using Kontecg.MultiCompany;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Used to configure multi-company.
    /// </summary>
    internal class MultiCompanyConfig : IMultiCompanyConfig
    {
        public MultiCompanyConfig()
        {
            Resolvers = new TypeList<ICompanyResolveContributor>();
            CompanyIdResolveKey = "Kontecg.CompanyId";
        }

        /// <summary>
        ///     Is multi-company enabled?
        ///     Default value: false.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Ignore feature check for host users
        ///     Default value: false.
        /// </summary>
        public bool IgnoreFeatureCheckForHostUsers { get; set; }

        public ITypeList<ICompanyResolveContributor> Resolvers { get; }

        public string CompanyIdResolveKey { get; set; }
    }
}
