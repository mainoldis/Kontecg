using System.Linq;
using System.Reflection;
using Kontecg.Modules;
using Kontecg.Reflection;
using Kontecg.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Modules
{
    public class KontecgAssemblyFinder_Tests: TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Get_Module_And_Additional_Assemblies()
        {
            //Arrange
            var bootstrapper = KontecgBootstrapper.Create<MyStartupModule>(options =>
            {
                options.IocManager = LocalIocManager;
            });

            bootstrapper.Initialize();

            //Act
            var assemblies = bootstrapper.IocManager.Resolve<KontecgAssemblyFinder>().GetAllAssemblies();

            //Assert
            assemblies.Count.ShouldBe(3);

            assemblies.Any(a => a == typeof(MyStartupModule).GetAssembly()).ShouldBeTrue();
            assemblies.Any(a => a == typeof(KontecgKernelModule).GetAssembly()).ShouldBeTrue();
            assemblies.Any(a => a == typeof(FactAttribute).GetAssembly()).ShouldBeTrue();
        }

        public class MyStartupModule : KontecgModule
        {
            public override Assembly[] GetAdditionalAssemblies()
            {
                return new[] {typeof(FactAttribute).GetAssembly()};
            }
        }
    }
}
