using Kontecg.Dependency;

namespace Kontecg.BlobStoring
{
    public class DefaultBlobContainerConfigurationProvider : IBlobContainerConfigurationProvider, ITransientDependency
    {
        protected KontecgBlobStoringOptions Options { get; }

        public DefaultBlobContainerConfigurationProvider(KontecgBlobStoringOptions options)
        {
            Options = options;
        }

        public virtual BlobContainerConfiguration Get(string name)
        {
            return Options.Containers.GetConfiguration(name);
        }
    }
}