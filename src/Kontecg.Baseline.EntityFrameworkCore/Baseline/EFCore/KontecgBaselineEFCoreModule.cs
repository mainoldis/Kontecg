using Castle.MicroKernel.Registration;
using Kontecg.Domain.Uow;
using Kontecg.EFCore;
using Kontecg.Modules;
using Kontecg.MultiCompany;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Baseline.EFCore
{
    /// <summary>
    ///     Entity framework integration module for Kontecg Baseline.
    /// </summary>
    [DependsOn(typeof(KontecgBaselineModule), typeof(KontecgEntityFrameworkCoreModule))]
    public class KontecgBaselineEFCoreModule : KontecgModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService(typeof(IConnectionStringResolver), () =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IConnectionStringResolver, IDbPerCompanyConnectionStringResolver>()
                        .ImplementedBy<DbPerCompanyConnectionStringResolver>()
                        .LifestyleTransient()
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgBaselineEFCoreModule).GetAssembly());
        }
    }
}
