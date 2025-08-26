using Kontecg.Hangfire.Configuration;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Hangfire;

namespace Kontecg.Hangfire
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgHangfireModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgHangfireConfiguration, KontecgHangfireConfiguration>();
            
            Configuration.Modules
                .UseHangfire()
                .GlobalConfiguration
                .UseActivator(new HangfireIocJobActivator(IocManager));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgHangfireModule).GetAssembly());
            GlobalJobFilters.Filters.Add(IocManager.Resolve<KontecgHangfireJobExceptionFilter>());
        }
    }
}
