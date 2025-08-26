using System.Reflection;
using Kontecg.Dependency;
using Kontecg.Modules;

namespace Kontecg.BlobStoring
{
    public class KontecgBlobStoringModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<KontecgBlobStoringOptions>();

            IocManager.Register(typeof(IBlobContainer<>), typeof(BlobContainer<>), DependencyLifeStyle.Transient);
            IocManager.Register<IBlobContainer, BlobContainer<DefaultContainer>>(DependencyLifeStyle.Transient);

        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
