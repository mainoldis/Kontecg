using Castle.MicroKernel.Registration;
using Kontecg.MachineLearning.Configuration;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.MachineLearning
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgMachineLearningModule : KontecgModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgMachineLearningConfiguration, KontecgMachineLearningConfiguration>();
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgMachineLearningModule).GetAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof(IMlContextProvider))
                    .ImplementedBy(typeof(DefaultMlContextProvider))
                    .LifestyleTransient()
            );
        }
    }
}
