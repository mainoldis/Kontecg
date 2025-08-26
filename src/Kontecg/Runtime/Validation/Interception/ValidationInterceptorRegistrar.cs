using System.Reflection;
using Castle.Core;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Validation.Interception
{
    internal static class ValidationInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                TypeInfo implementationType = handler.ComponentModel.Implementation.GetTypeInfo();

                if (!iocManager.IsRegistered<IKontecgValidationDefaultOptions>())
                {
                    return;
                }

                IKontecgValidationDefaultOptions validationOptions =
                    iocManager.Resolve<IKontecgValidationDefaultOptions>();

                if (validationOptions.IsConventionalValidationClass(implementationType.AsType()))
                {
                    handler.ComponentModel.Interceptors.Add(
                        new InterceptorReference(typeof(KontecgAsyncDeterminationInterceptor<ValidationInterceptor>)));
                }
            };
        }
    }
}
