using Kontecg.Castle.MsAdapter;
using Kontecg.MassTransit.Abstractions;
using Kontecg.MassTransit.Configuration;
using Kontecg.MassTransit.Mappers;
using Kontecg.MassTransit.Strategies;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.MassTransit
{
    [DependsOn(typeof(KontecgKernelModule), typeof(KontecgCastleMsAdapterModule))]
    public class KontecgMassTransitModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgMassTransitModuleConfiguration, KontecgMassTransitModuleConfiguration>();
            IocManager.Register<IEventMessageMapper, DefaultEventMessageMapper>();
            IocManager.Register<IEventPublishingStrategy, AttributeBasedPublishingStrategy>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgMassTransitModule).GetAssembly());
        }
    }
}
