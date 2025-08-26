using Kontecg.Collections;
using Kontecg.MultiCompany;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Used to configure multi-company.
    /// </summary>
    public interface IMultiCompanyConfig
    {
        /// <summary>
        ///     Is multi-company enabled?
        ///     Default value: false.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        ///     Ignore feature check for host users
        ///     Default value: false.
        /// </summary>
        bool IgnoreFeatureCheckForHostUsers { get; set; }

        /// <summary>
        ///     A list of contributors for company resolve process.
        /// </summary>
        ITypeList<ICompanyResolveContributor> Resolvers { get; }

        /// <summary>
        ///     CompanyId resolve key
        ///     Default value: "Kontecg.CompanyId"
        /// </summary>
        string CompanyIdResolveKey { get; set; }
    }
}
