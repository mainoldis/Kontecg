namespace Kontecg.MongoDb.Configuration
{
    public interface IKontecgMongoDbModuleConfiguration
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }
}
