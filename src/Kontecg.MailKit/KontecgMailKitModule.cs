using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Modules;
using Kontecg.Net.Mail;
using Kontecg.Reflection.Extensions;

namespace Kontecg.MailKit
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgMailKitModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgMailKitConfiguration, KontecgMailKitConfiguration>();
            Configuration.ReplaceService<IEmailSender, MailKitEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgMailKitModule).GetAssembly());
        }
    }
}
