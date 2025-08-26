using Castle.MicroKernel.Lifestyle.Scoped;

namespace Kontecg.Castle.MsAdapter
{
    public interface IMsLifetimeScope
    {
        ILifetimeScope WindsorLifeTimeScope { get; }

        void AddInstance(object instance);

        void AddChild(MsLifetimeScope lifetimeScope);

        void RemoveChild(MsLifetimeScope lifetimeScope);

        void Dispose();
    }
}
