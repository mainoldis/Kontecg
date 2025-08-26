using Kontecg.Collections;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Internal implementation for <see cref="IFeatureConfiguration" />.
    /// </summary>
    internal class FeatureConfiguration : IFeatureConfiguration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureConfiguration" /> class.
        /// </summary>
        public FeatureConfiguration()
        {
            Providers = new TypeList<FeatureProvider>();
        }

        /// <summary>
        ///     Reference to the feature providers.
        /// </summary>
        public ITypeList<FeatureProvider> Providers { get; }
    }
}
