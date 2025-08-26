using Kontecg.MemoryDb.Configuration;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.MemoryDb
{
    /// <summary>
    ///     This module is used to implement "Data Access Layer" in MemoryDb.
    /// </summary>
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgMemoryDbModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgMemoryDbModuleConfiguration, KontecgMemoryDbModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgMemoryDbModule).GetAssembly());
        }
    }
}
