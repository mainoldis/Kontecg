using Kontecg.MongoDb.Configuration;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Defines extension methods to <see cref="IModuleConfigurations" /> to allow to configure Kontecg MongoDb module.
    /// </summary>
    public static class KontecgMongoDbConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure Kontecg MongoDb module.
        /// </summary>
        public static IKontecgMongoDbModuleConfiguration UseMongoDb(this IModuleConfigurations configurations)
        {
            return configurations.KontecgConfiguration.Get<IKontecgMongoDbModuleConfiguration>();
        }
    }
}
