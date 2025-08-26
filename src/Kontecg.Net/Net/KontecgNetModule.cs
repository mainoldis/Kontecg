using Kontecg.Modules;
using Kontecg.Net.Configuration;
using Kontecg.Net.Security;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Net
{
    /// <summary>
    ///     This module is used to implement Networking.
    /// </summary>
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgNetModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgNetModuleConfiguration, KontecgNetModuleConfiguration>();
            IocManager.Register<ICertificateProvider, SimpleCertificateProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgNetModule).GetAssembly());
        }
    }
}
