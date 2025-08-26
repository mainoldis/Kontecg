using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.Workflows;

namespace Kontecg.EFCore
{
    [DependsOn(
        typeof(KontecgWorkflowModule),
        typeof(KontecgEntityFrameworkCoreModule))]
    public class KontecgWorkflowsEntityFrameworkCore : KontecgModule
    {
        /* Used it tests to skip DbContext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {

            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgWorkflowsEntityFrameworkCore).GetAssembly());
        }
    }
}
