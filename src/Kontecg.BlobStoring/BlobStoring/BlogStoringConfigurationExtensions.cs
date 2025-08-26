using Kontecg.Configuration.Startup;

namespace Kontecg.BlobStoring
{
    public static class BlogStoringConfigurationExtensions
    {
        public static KontecgBlobStoringOptions UseBlobStoring(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<KontecgBlobStoringOptions>();
        }
    }
}
