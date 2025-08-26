using Castle.Windsor;

namespace Kontecg.Castle.MsAdapter
{
    public class GlobalMsLifetimeScope : MsLifetimeScope
    {
        public GlobalMsLifetimeScope(IWindsorContainer container)
            : base(container)
        {
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            Container.Dispose();
        }
    }
}
