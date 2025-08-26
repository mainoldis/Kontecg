using System.Linq;
using Kontecg.Localization;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Localization
{
    public class DefaultLanguageProvider_Tests : TestBaseWithLocalIocManager
    {
        public DefaultLanguageProvider_Tests()
        {
            LocalIocManager.Register<ILanguageProvider, DefaultLanguageProvider>();
            var bootstrapper = KontecgBootstrapper.Create<DefaultLanguageProviderLangModule>(options =>
            {
                options.IocManager = LocalIocManager;
            });

            bootstrapper.Initialize();
        }

        [Fact]
        public void Should_Get_Languages()
        {
            var languageProvider = LocalIocManager.Resolve<ILanguageProvider>();

            var allLanguages = languageProvider.GetLanguages();
            allLanguages.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_Active_Languages()
        {
            var languageProvider = LocalIocManager.Resolve<ILanguageProvider>();

            var activeLanguages = languageProvider.GetActiveLanguages();
            activeLanguages.Count.ShouldBe(1);
            activeLanguages.Single().Name.ShouldBe("en");
        }
    }

    public class DefaultLanguageProviderLangModule : KontecgModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", isDefault: true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", isDisabled: true));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DefaultLanguageProviderLangModule).GetAssembly());
        }
    }
}
