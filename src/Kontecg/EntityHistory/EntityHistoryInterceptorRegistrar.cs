using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Kontecg.Dependency;

namespace Kontecg.EntityHistory
{
    internal static class EntityHistoryInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                if (!iocManager.IsRegistered<IEntityHistoryConfiguration>())
                {
                    return;
                }

                IEntityHistoryConfiguration entityHistoryConfiguration =
                    iocManager.Resolve<IEntityHistoryConfiguration>();

                if (ShouldIntercept(entityHistoryConfiguration, handler.ComponentModel.Implementation))
                {
                    handler.ComponentModel.Interceptors.Add(
                        new InterceptorReference(
                            typeof(KontecgAsyncDeterminationInterceptor<EntityHistoryInterceptor>)));
                }
            };
        }

        private static bool ShouldIntercept(IEntityHistoryConfiguration entityHistoryConfiguration, Type type)
        {
            return type.GetTypeInfo().IsDefined(typeof(UseCaseAttribute), true) ||
                   type.GetMethods().Any(m => m.IsDefined(typeof(UseCaseAttribute), true));
        }
    }
}
