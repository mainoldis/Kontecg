using System.Reflection;
using Kontecg.Modules;
using Kontecg.MongoDb.Configuration;

namespace Kontecg.MongoDb
{
    /// <summary>
    ///     This module is used to implement "Data Access Layer" in MongoDB.
    /// </summary>
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgMongoDbModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgMongoDbModuleConfiguration, KontecgMongoDbModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
