using System;
using System.Threading.Tasks;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Runtime.Session;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Default implementation for <see cref="IFeatureChecker" />.
    /// </summary>
    public class FeatureChecker : IFeatureChecker, ITransientDependency, IIocManagerAccessor
    {
        private readonly IFeatureManager _featureManager;
        private readonly IMultiCompanyConfig _multiCompanyConfig;

        /// <summary>
        ///     Creates a new <see cref="FeatureChecker" /> object.
        /// </summary>
        public FeatureChecker(IFeatureManager featureManager, IMultiCompanyConfig multiCompanyConfig)
        {
            _featureManager = featureManager;
            _multiCompanyConfig = multiCompanyConfig;

            FeatureValueStore = NullFeatureValueStore.Instance;
            KontecgSession = NullKontecgSession.Instance;
        }

        /// <summary>
        ///     Reference to the current session.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        /// <summary>
        ///     Reference to the store used to get feature values.
        /// </summary>
        public IFeatureValueStore FeatureValueStore { get; set; }

        /// <inheritdoc />
        public Task<string> GetValueAsync(string name)
        {
            if (KontecgSession.CompanyId == null)
            {
                throw new KontecgException(
                    "FeatureChecker can not get a feature value by name. CompanyId is not set in the IKontecgSession!");
            }

            return GetValueAsync(KontecgSession.CompanyId.Value, name);
        }

        /// <inheritdoc />
        public string GetValue(string name)
        {
            if (KontecgSession.CompanyId == null)
            {
                throw new KontecgException(
                    "FeatureChecker can not get a feature value by name. CompanyId is not set in the IKontecgSession!");
            }

            return GetValue(KontecgSession.CompanyId.Value, name);
        }

        /// <inheritdoc />
        public async Task<string> GetValueAsync(int companyId, string name)
        {
            Feature feature = _featureManager.Get(name);
            string value = await FeatureValueStore.GetValueOrNullAsync(companyId, feature);

            return value ?? feature.DefaultValue;
        }

        /// <inheritdoc />
        public string GetValue(int companyId, string name)
        {
            Feature feature = _featureManager.Get(name);
            string value = FeatureValueStore.GetValueOrNull(companyId, feature);

            return value ?? feature.DefaultValue;
        }

        /// <inheritdoc />
        public async Task<bool> IsEnabledAsync(string featureName)
        {
            if (KontecgSession.CompanyId == null && _multiCompanyConfig.IgnoreFeatureCheckForHostUsers)
            {
                return true;
            }

            return string.Equals(await GetValueAsync(featureName), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public bool IsEnabled(string featureName)
        {
            if (KontecgSession.CompanyId == null && _multiCompanyConfig.IgnoreFeatureCheckForHostUsers)
            {
                return true;
            }

            return string.Equals(GetValue(featureName), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public async Task<bool> IsEnabledAsync(int companyId, string featureName)
        {
            return string.Equals(await GetValueAsync(companyId, featureName), "true",
                StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public bool IsEnabled(int companyId, string featureName)
        {
            return string.Equals(GetValue(companyId, featureName), "true", StringComparison.OrdinalIgnoreCase);
        }

        public IIocManager IocManager { get; set; }
    }
}
