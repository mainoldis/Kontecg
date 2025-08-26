using FluentValidation;
using Kontecg.Dependency;
using Kontecg.FluentValidation.Configuration;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;

namespace Kontecg.FluentValidation
{
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgFluentValidationModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgFluentValidationConfiguration, KontecgFluentValidationConfiguration>();
            IocManager.Register<KontecgFluentValidationLanguageManager, KontecgFluentValidationLanguageManager>();
            IocManager.Register<IValidatorFactory, KontecgFluentValidationValidatorFactory>(DependencyLifeStyle
                .Transient);

            IocManager.AddConventionalRegistrar(new FluentValidationValidatorRegistrar());

            Configuration.Validation.Validators.Add<FluentValidationMethodParameterValidator>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgFluentValidationModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            ValidatorOptions.Global.LanguageManager = IocManager.Resolve<KontecgFluentValidationLanguageManager>();
        }
    }
}
