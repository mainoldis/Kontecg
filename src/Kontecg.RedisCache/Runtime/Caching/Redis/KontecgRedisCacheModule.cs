using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Runtime.Caching.Redis
{
    /// <summary>
    /// This modules is used to replace Kontecg's cache system with Redis server.
    /// </summary>
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgRedisCacheModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<KontecgRedisCacheOptions>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgRedisCacheModule).GetAssembly());
        }
    }
}
