using System;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.Castle.MsAdapter
{
    /// <summary>
    ///     Implements <see cref="IServiceScope" />.
    /// </summary>
    public class WindsorServiceScope : IServiceScope
    {
        private readonly IWindsorContainer _container;
        private readonly IMsLifetimeScope _parentLifetimeScope;

        public WindsorServiceScope(IWindsorContainer container, IMsLifetimeScope currentMsLifetimeScope)
        {
            _container = container;
            _parentLifetimeScope = currentMsLifetimeScope;

            LifetimeScope = new MsLifetimeScope(container);

            _parentLifetimeScope?.AddChild(LifetimeScope);

            using (MsLifetimeScope.Using(LifetimeScope))
            {
                ServiceProvider = container.Resolve<IServiceProvider>();
            }
        }

        public MsLifetimeScope LifetimeScope { get; }
        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            _parentLifetimeScope?.RemoveChild(LifetimeScope);
            LifetimeScope.Dispose();
            _container.Release(ServiceProvider);
        }
    }
}
