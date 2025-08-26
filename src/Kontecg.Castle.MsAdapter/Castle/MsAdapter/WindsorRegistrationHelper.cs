﻿using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

namespace Kontecg.Castle.MsAdapter
{
    public static class WindsorRegistrationHelper
    {
        /// <summary>
        /// Adds all given services from <see cref="Microsoft.Extensions.DependencyInjection"/>
        /// to the Castle Windsor container.
        /// </summary>
        /// <returns>A Castle Windsor service provider.</returns>
        public static IServiceProvider CreateServiceProvider(IWindsorContainer container, IServiceCollection services)
        {
            AddServices(container, services);

            using (MsLifetimeScope.Using(container.Resolve<GlobalMsLifetimeScope>()))
            {
                return container.Resolve<GlobalScopedWindsorServiceProvider>();
            }
        }

        /// <summary>
        /// Used to add services to an existing container, without creating a <see cref="IServiceProvider"/>.
        /// </summary>
        public static void AddServices(this IWindsorContainer container, IServiceCollection services)
        {
            container.Install(new ServiceCollectionInstaller(services));
        }

        internal static void AddServicesInternal(this IWindsorContainer container, IServiceCollection services)
        {
            AddBaseServices(container);
            AddSubResolvers(container);
            AddServicesCollection(container, services);
        }

        private static void AddBaseServices(IWindsorContainer container)
        {
            if (!container.Kernel.HasComponent(typeof(IWindsorContainer)))
            {
                container.Register(
                    Component.For<IWindsorContainer>()
                        .Instance(container)
                        .LifestyleSingleton()
                );
            }

            if (!container.Kernel.HasComponent(typeof(GlobalMsLifetimeScope)))
            {
                container.Register(
                    Component.For<GlobalMsLifetimeScope>()
                        .LifestyleSingleton()
                );
            }

            if (!container.Kernel.HasComponent(typeof(MsLifetimeScopeProvider)))
            {
                container.Register(
                    Component.For<MsLifetimeScopeProvider>()
                        .LifestyleTransient()
                );
            }

            if (!container.Kernel.HasComponent(typeof(IServiceScopeFactory)))
            {
                container.Register(
                    Component.For<IServiceScopeFactory>()
                        .ImplementedBy<WindsorServiceScopeFactory>()
                        .LifestyleSingleton()
                );
            }

            if (!container.Kernel.HasComponent(typeof(IServiceProvider)))
            {
                container.Register(
                    Component.For<IServiceProvider, IServiceProviderIsService>()
                        .ImplementedBy<ScopedWindsorServiceProvider>()
                        .LifestyleTransient()
                );
            }

            if (!container.Kernel.HasComponent(typeof(GlobalScopedWindsorServiceProvider)))
            {
                container.Register(
                    Component.For<GlobalScopedWindsorServiceProvider>()
                        .LifestyleTransient()
                );
            }
        }

        private static void AddSubResolvers(IWindsorContainer container)
        {
            // ASP.NET Core uses IEnumerable<T> to resolve a list of types.
            // Since some of these types are optional, Windsor must also return empty collections.
            container.Kernel.Resolver.AddSubResolver(new MsCompatibleCollectionResolver(container.Kernel));

            //Workaround for Options resolve problem. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/1563#issuecomment-261654317
            container.Kernel.Resolver.AddSubResolver(new MsOptionsSubResolver(container.Kernel));
        }

        private static void AddServicesCollection(IWindsorContainer container, IServiceCollection services)
        {
            foreach (var serviceDescriptor in services)
            {
                if (serviceDescriptor.ImplementationInstance == container)
                {
                    //Already registered before
                    continue;
                }

                RegisterServiceDescriptor(container, serviceDescriptor);
            }
        }

        private static void RegisterServiceDescriptor(IWindsorContainer container, ServiceDescriptor serviceDescriptor)
        {
            // MS allows the same type to be registered multiple times.
            // Castle Windsor throws an exception in that case - it requires an unique name.
            // For that reason, we use Guids to ensure uniqueness.
            string uniqueName = serviceDescriptor.ServiceType.FullName + "_" + Guid.NewGuid();

            // The IsDefault() calls are important because this makes sure that the last service
            // is returned in case of multiple registrations. This is by design in the MS library:
            // https://github.com/aspnet/DependencyInjection/blob/dev/src/Microsoft.Extensions.DependencyInjection.Specification.Tests/DependencyInjectionSpecificationTests.cs#L254

            if (serviceDescriptor.ImplementationType != null)
            {
                container.Register(
                    Component.For(serviceDescriptor.ServiceType)
                        .Named(uniqueName)
                        .IsDefault()
                        .ImplementedBy(serviceDescriptor.ImplementationType)
                        .ConfigureLifecycle(serviceDescriptor.Lifetime));
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                var serviceDescriptorRef = serviceDescriptor;
                container.Register(
                    Component.For(serviceDescriptor.ServiceType)
                        .Named(uniqueName)
                        .IsDefault()
                        .UsingFactoryMethod(c => serviceDescriptorRef.ImplementationFactory(c.Resolve<IServiceProvider>()))
                        .ConfigureLifecycle(serviceDescriptor.Lifetime)
                    );
            }
            else
            {
                container.Register(
                    Component.For(serviceDescriptor.ServiceType)
                        .Named(uniqueName)
                        .IsDefault()
                        .Instance(serviceDescriptor.ImplementationInstance)
                        .ConfigureLifecycle(serviceDescriptor.Lifetime)
                    );
            }
        }

        private static ComponentRegistration<object> ConfigureLifecycle(this ComponentRegistration<object> registrationBuilder, ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Transient:
                    registrationBuilder.LifestyleTransient();
                    break;
                case ServiceLifetime.Scoped:
                    registrationBuilder.LifestyleCustom<MsScopedLifestyleManager>();
                    break;
                case ServiceLifetime.Singleton:
                    registrationBuilder.LifestyleSingleton();
                    break;
                default:
                    throw new NotImplementedException("Unknown ServiceLifetime: " + serviceLifetime);
            }

            return registrationBuilder;
        }
    }

    internal sealed class ServiceCollectionInstaller : IWindsorInstaller
    {
        private readonly IServiceCollection _services;

        public ServiceCollectionInstaller(IServiceCollection services)
        {
            _services = services;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddServicesInternal(_services);
        }
    }
}
