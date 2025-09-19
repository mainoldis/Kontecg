using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.MassTransit.Abstractions;
using Kontecg.MassTransit.Mappers;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.TestBase;
using MassTransit;

namespace Kontecg.MassTransit.Tests
{
    [DependsOn(typeof(KontecgMassTransitModule), typeof(KontecgTestBaseModule))]
    public class KontecgMassTransitTestModule : KontecgModule
    {
        private IBusControl _busControl;

        public override void PreInitialize()
        {
            Configuration.EventBus.UseDefaultEventBus = true;
            Configuration.ReplaceService<IEventMessageMapper, DirectEventMessageMapper>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgMassTransitTestModule).GetAssembly());
            //_busControl = MassTransitRegistrar.RegisterUsingRabbitMq(IocManager, new DirectEventMessageMapper());
            _busControl = MassTransitRegistrar.RegisterUsingInMemory(IocManager);
        }

        public override void PostInitialize()
        {
            _busControl?.Start();
        }

        /// <inheritdoc />
        public override void Shutdown()
        {
            _busControl.Stop();
        }
    }
}
