using Elsa.Workflows.Runtime;
using Kontecg.Dependency;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.Workflows.Configuration;

namespace Kontecg.Workflows
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgWorkflowModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgWorkflowConfiguration, KontecgWorkflowConfiguration>();
            IocManager.Register<IWorkflowsProvider, KontecgWorkflowProvider>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgWorkflowModule).GetAssembly());
        }
    }
}
