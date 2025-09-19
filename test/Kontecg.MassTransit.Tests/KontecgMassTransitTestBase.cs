using Kontecg.TestBase;
using Kontecg.Timing;

namespace Kontecg.MassTransit.Tests
{
    public abstract class KontecgMassTransitTestBase : KontecgIntegratedTestBase<KontecgMassTransitTestModule>
    {
        protected KontecgMassTransitTestBase()
        {
            Clock.Provider = ClockProviders.Utc;
        }
    }
}
