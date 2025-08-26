using System;
using Kontecg.Dependency;
using Kontecg.Runtime.Session;
using Kontecg.Threading;

namespace Kontecg.BlobStoring
{
    public class BlobContainerFactory : IBlobContainerFactory, ITransientDependency
    {
        protected IBlobProviderSelector ProviderSelector { get; }

        protected IBlobContainerConfigurationProvider ConfigurationProvider { get; }

        protected IKontecgSession CurrentCompany { get; }

        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected IBlobNormalizeNamingService BlobNormalizeNamingService { get; }

        public BlobContainerFactory(
            IBlobContainerConfigurationProvider configurationProvider,
            IKontecgSession currentCompany,
            ICancellationTokenProvider cancellationTokenProvider,
            IBlobProviderSelector providerSelector,
            IServiceProvider serviceProvider,
            IBlobNormalizeNamingService blobNormalizeNamingService)
        {
            ConfigurationProvider = configurationProvider;
            CurrentCompany = currentCompany;
            CancellationTokenProvider = cancellationTokenProvider;
            ProviderSelector = providerSelector;
            ServiceProvider = serviceProvider;
            BlobNormalizeNamingService = blobNormalizeNamingService;
        }

        public virtual IBlobContainer Create(string name)
        {
            var configuration = ConfigurationProvider.Get(name);

            return new BlobContainer(
                name,
                configuration,
                ProviderSelector.Get(name),
                CurrentCompany,
                CancellationTokenProvider,
                BlobNormalizeNamingService,
                ServiceProvider
            );
        }
    }

}
