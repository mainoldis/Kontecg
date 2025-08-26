using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Runtime.Session;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Dependency
{
    public class PropertyInjection_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Inject_Session_For_ApplicationService()
        {
            var session = Substitute.For<IKontecgSession>();
            session.CompanyId.Returns(1);
            session.UserId.Returns(42);

            LocalIocManager.Register<MyApplicationService>();
            LocalIocManager.IocContainer.Register(
                Component.For<IKontecgSession>().Instance(session)
                );

            var myAppService = LocalIocManager.Resolve<MyApplicationService>();
            myAppService.TestSession();
        }

        private class MyApplicationService : ApplicationService
        {
            public void TestSession()
            {
                KontecgSession.ShouldNotBe(null);
                KontecgSession.CompanyId.ShouldBe(1);
                KontecgSession.UserId.ShouldBe(42);
            }
        }
    }
}
