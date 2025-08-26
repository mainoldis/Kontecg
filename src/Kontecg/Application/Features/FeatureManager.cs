using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kontecg.Dependency;

namespace Kontecg.Application.Features
{
    /// <summary>
    ///     Implements <see cref="IFeatureManager" />.
    /// </summary>
    internal class FeatureManager : FeatureDefinitionContextBase, IFeatureManager, ISingletonDependency
    {
        private readonly IFeatureConfiguration _featureConfiguration;
        private readonly IIocManager _iocManager;

        /// <summary>
        ///     Creates a new <see cref="FeatureManager" /> object
        /// </summary>
        /// <param name="iocManager">IOC Manager</param>
        /// <param name="featureConfiguration">Feature configuration</param>
        public FeatureManager(IIocManager iocManager, IFeatureConfiguration featureConfiguration)
        {
            _iocManager = iocManager;
            _featureConfiguration = featureConfiguration;
        }

        /// <summary>
        ///     Gets a feature by its given name
        /// </summary>
        /// <param name="name">Name of the feature</param>
        public Feature Get(string name)
        {
            Feature feature = GetOrNull(name);
            if (feature == null)
            {
                throw new KontecgException("There is no feature with name: " + name);
            }

            return feature;
        }

        /// <summary>
        ///     Gets all the features
        /// </summary>
        public IReadOnlyList<Feature> GetAll()
        {
            return Features.Values.ToImmutableList();
        }

        /// <summary>
        ///     Initializes this <see cref="FeatureManager" />
        /// </summary>
        public void Initialize()
        {
            foreach (Type providerType in _featureConfiguration.Providers)
            {
                using IDisposableDependencyObjectWrapper<FeatureProvider> provider = CreateProvider(providerType);
                provider.Object.SetFeatures(this);
            }

            Features.AddAllFeatures();
        }

        private IDisposableDependencyObjectWrapper<FeatureProvider> CreateProvider(Type providerType)
        {
            return _iocManager.ResolveAsDisposable<FeatureProvider>(providerType);
        }
    }
}
