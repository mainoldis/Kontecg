namespace Kontecg.MongoDb.Configuration
{
    internal class KontecgMongoDbModuleConfiguration : IKontecgMongoDbModuleConfiguration
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}
