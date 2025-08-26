using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.Castle.MsAdapter
{
    public static class ServiceCollectionExtensions
    {
        public static T GetSingletonServiceOrNull<T>(this IServiceCollection services)
        {
            return (T) services
                .FirstOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }

        public static T GetSingletonService<T>(this IServiceCollection services)
        {
            T service = services.GetSingletonServiceOrNull<T>();
            if (service == null)
            {
                throw new Exception("Can not find service: " + typeof(T).AssemblyQualifiedName);
            }

            return service;
        }
    }
}
