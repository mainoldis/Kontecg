using Castle.MicroKernel.Registration;
using FluentValidation;
using Kontecg.Dependency;

namespace Kontecg.FluentValidation
{
    public class FluentValidationValidatorRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn(typeof(IValidator<>)).WithService.Base()
                    .LifestyleTransient()
            );
        }
    }
}
