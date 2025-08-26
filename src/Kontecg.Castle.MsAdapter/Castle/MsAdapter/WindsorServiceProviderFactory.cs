using System;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.Castle.MsAdapter
{
    public class WindsorServiceProviderFactory : IServiceProviderFactory<IWindsorContainer>
    {
        public IWindsorContainer CreateBuilder(IServiceCollection services)
        {
            IWindsorContainer container = services.GetSingletonServiceOrNull<IWindsorContainer>();

            if (container == null)
            {
                container = new WindsorContainer();
                services.AddSingleton(container);
            }

            container.AddServices(services);

            return container;
        }

        public IServiceProvider CreateServiceProvider(IWindsorContainer containerBuilder)
        {
            return containerBuilder.Resolve<IServiceProvider>();
        }
    }
}
