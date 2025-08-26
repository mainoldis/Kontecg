using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Kontecg.Dependency;

namespace Kontecg.ExceptionHandling
{
    internal static class ExceptionHandlerInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                if (ShouldIntercept(iocManager, handler.ComponentModel.Implementation))
                    handler.ComponentModel.Interceptors.Add(
                        new InterceptorReference(typeof(KontecgAsyncDeterminationInterceptor<ExceptionHandlerInterceptor>)));
            };
        }

        private static bool ShouldIntercept(IIocManager iocManager, Type type)
        {
            if (type.GetTypeInfo().IsDefined(typeof(ExceptionHandleAttribute), true)) return true;

            if (type.GetMethods().Any(m => m.IsDefined(typeof(DisableExceptionHandlingAttribute), true))) return false;

            if (type.GetMethods().Any(m => m.IsDefined(typeof(ExceptionHandleAttribute), true))) return true;

            if (!iocManager.IsRegistered<IKontecgExceptionHandlingDefaultOptions>()) return false;

            var exceptionHandlingOptions = iocManager.Resolve<IKontecgExceptionHandlingDefaultOptions>();

            if (exceptionHandlingOptions.ConventionalExceptionHandlingSelectors.Any(selector => selector(type))) return true;

            return false;
        }
    }
}
