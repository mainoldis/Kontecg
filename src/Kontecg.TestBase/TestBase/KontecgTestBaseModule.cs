using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.TestBase
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgTestBaseModule : KontecgModule
    {
        public override void PreInitialize()
        {
            Configuration.EventBus.UseDefaultEventBus = false;
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgTestBaseModule).GetAssembly());
        }
    }
}
