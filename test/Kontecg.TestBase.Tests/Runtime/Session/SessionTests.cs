using Kontecg.Configuration.Startup;
using Kontecg.Runtime.Session;
using Shouldly;
using Xunit;

namespace Kontecg.TestBase.Tests.Runtime.Session
{
    public class SessionTests : KontecgIntegratedTestBase<KontecgKernelModule>
    {
        [Fact]
        public void Should_Be_Default_On_Startup()
        {
            Resolve<IMultiCompanyConfig>().IsEnabled = false;

            KontecgSession.UserId.ShouldBe(null);
            KontecgSession.CompanyId.ShouldBe(1);

            Resolve<IMultiCompanyConfig>().IsEnabled = true;

            KontecgSession.UserId.ShouldBe(null);
            KontecgSession.CompanyId.ShouldBe(null);
        }

        [Fact]
        public void Can_Change_Session_Variables()
        {
            Resolve<IMultiCompanyConfig>().IsEnabled = true;

            KontecgSession.UserId = 1;
            KontecgSession.CompanyId = 42;

            var resolvedKontecgSession = LocalIocManager.Resolve<IKontecgSession>();

            resolvedKontecgSession.UserId.ShouldBe(1);
            resolvedKontecgSession.CompanyId.ShouldBe(42);

            Resolve<IMultiCompanyConfig>().IsEnabled = false;

            KontecgSession.UserId.ShouldBe(1);
            KontecgSession.CompanyId.ShouldBe(1);
        }
    }
}
