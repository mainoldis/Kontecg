using Kontecg.Application.Features;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Feature setting for a Company (<see cref="KontecgCompany{TUser}" />).
    /// </summary>
    public class CompanyFeatureSetting : FeatureSetting
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyFeatureSetting" /> class.
        /// </summary>
        public CompanyFeatureSetting()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyFeatureSetting" /> class.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="name">Feature name.</param>
        /// <param name="value">Feature value.</param>
        public CompanyFeatureSetting(int companyId, string name, string value)
            : base(name, value)
        {
            CompanyId = companyId;
        }
    }
}
