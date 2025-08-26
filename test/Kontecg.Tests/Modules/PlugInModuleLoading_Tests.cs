using System.Linq;
using Kontecg.Modules;
using Kontecg.PlugIns;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Modules
{
    public class PlugInModuleLoading_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Load_All_Modules()
        {
            //Arrange
            var bootstrapper = KontecgBootstrapper.Create<MyStartupModule>(options =>
            {
                options.IocManager = LocalIocManager;
            });

            bootstrapper.PlugInSources.AddTypeList(typeof(MyPlugInModule));

            bootstrapper.Initialize();

            //Act
            var modules = bootstrapper.IocManager.Resolve<IKontecgModuleManager>().Modules;

            //Assert
            modules.Count.ShouldBe(6);

            modules.Any(m => m.Type == typeof(KontecgKernelModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyStartupModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule1)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyModule2)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInModule)).ShouldBeTrue();
            modules.Any(m => m.Type == typeof(MyPlugInDependedModule)).ShouldBeTrue();

            modules.Any(m => m.Type == typeof(MyNotDependedModule)).ShouldBeFalse();
        }

        [DependsOn(typeof(MyModule1), typeof(MyModule2))]
        public class MyStartupModule: KontecgModule
        {

        }

        public class MyModule1 : KontecgModule
        {
            
        }

        public class MyModule2 : KontecgModule
        {

        }
        
        public class MyNotDependedModule : KontecgModule
        {

        }

        [DependsOn(typeof(MyPlugInDependedModule))]
        public class MyPlugInModule : KontecgModule
        {
            
        }

        public class MyPlugInDependedModule : KontecgModule
        {
            
        }
    }
}
