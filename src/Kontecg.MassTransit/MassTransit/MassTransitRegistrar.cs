using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Kontecg.Castle.MsAdapter;
using Kontecg.Dependency;
using Kontecg.Events.Bus;
using Kontecg.MassTransit.Abstractions;
using Kontecg.MassTransit.Configuration;
using Kontecg.MassTransit.NamingConventions;
using Kontecg.Reflection;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DependencyRegistration = Castle.MicroKernel.Registration.Dependency;

namespace Kontecg.MassTransit
{
    public static class MassTransitRegistrar
    {
        public static IBusControl RegisterUsingRabbitMq(IIocManager iocManager, IEventMessageMapper eventMessageMapper = null,
            IEventPublishingStrategy eventPublishingStrategy = null, Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> configAction = null, IServiceCollection services = null)
        {
            var moduleConfiguration = iocManager.Resolve<IKontecgMassTransitModuleConfiguration>();
            var typeFinder = iocManager.Resolve<ITypeFinder>();

            var consumersTypes = typeFinder.Find(
                type =>
                {
                    TypeInfo typeInfo = type.GetTypeInfo();
                    return typeInfo.IsPublic &&
                           !typeInfo.IsAbstract &&
                           typeInfo.IsClass &&
                           typeof(IConsumer).IsAssignableFrom(type);
                });

            services ??= new ServiceCollection();

            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumers(consumersTypes);

                if (configAction != null)
                    configurator.UsingRabbitMq(configAction);
                else
                    configurator.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(moduleConfiguration.Options.Host, host =>
                        {
                            host.Username(moduleConfiguration.Options.Username);
                            host.Password(moduleConfiguration.Options.Password);
                            if (!string.IsNullOrEmpty(moduleConfiguration.Options.VirtualHost))
                                host.Heartbeat(TimeSpan.FromSeconds(moduleConfiguration.Options.HeartbeatInterval));
                        });

                        cfg.AddSerializer(new SystemTextJsonMessageSerializerFactory());

                        cfg.ConfigureEndpoints(ctx,
                            new KontecgEndpointNameFormatter(moduleConfiguration.Options.QueueName, false));

                        // Retry configuration for robustness
                        cfg.UseMessageRetry(retry =>
                        {
                            retry.Interval(3, TimeSpan.FromSeconds(5));
                            retry.Handle<Exception>();
                        });
                    });

                configurator.RemoveMassTransitHostedService();
            });

            iocManager.IocContainer.AddServices(services);

            var originalEventBus = iocManager.Resolve<IEventBus>();
            var messageMapper = eventMessageMapper ?? iocManager.Resolve<IEventMessageMapper>();
            var publishingStrategy = eventPublishingStrategy ?? iocManager.Resolve<IEventPublishingStrategy>();
            var busControl = iocManager.Resolve<IBusControl>();

            if (moduleConfiguration.UseDefaultEventBus)
            {
                var distributedEventBus = new DistributedEventBus(
                    originalEventBus,
                    busControl,
                    publishingStrategy,
                    messageMapper);

                iocManager.IocContainer.Register(
                    Component.For<IEventBus, IDistributedEventBus>()
                             .Instance(distributedEventBus)
                             .Named("distributed-event-bus")
                             .IsDefault()
                             .LifestyleSingleton());
            }
            else
            {
                iocManager.IocContainer.Register(
                    Component.For<IEventBus>()
                             .ImplementedBy<DistributedEventBus>()
                             .DependsOn(DependencyRegistration.OnComponent(typeof(IEventBus), "event-bus"))
                             .Named("distributed-event-bus")
                             .IsDefault()
                             .LifestyleSingleton()
                );
            }

            return busControl;
        }

        public static IBusControl RegisterUsingInMemory(IIocManager iocManager,
            IEventPublishingStrategy eventPublishingStrategy = null, IServiceCollection services = null)
        {
            var moduleConfiguration = iocManager.Resolve<IKontecgMassTransitModuleConfiguration>();
            var typeFinder = iocManager.Resolve<ITypeFinder>();

            var consumersTypes = typeFinder.Find(
                type =>
                {
                    TypeInfo typeInfo = type.GetTypeInfo();
                    return typeInfo.IsPublic &&
                           !typeInfo.IsAbstract &&
                           typeInfo.IsClass &&
                           typeof(IConsumer).IsAssignableFrom(type);
                });

            services ??= new ServiceCollection();

            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumers(consumersTypes);
                configurator.UsingInMemory((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                });

                configurator.RemoveMassTransitHostedService();
            });

            iocManager.IocContainer.AddServices(services);

            var originalEventBus = iocManager.Resolve<IEventBus>();
            var messageMapper = iocManager.Resolve<IEventMessageMapper>();
            var publishingStrategy = eventPublishingStrategy ?? iocManager.Resolve<IEventPublishingStrategy>();
            var busControl = iocManager.Resolve<IBusControl>();

            if (moduleConfiguration.UseDefaultEventBus)
            {
                var distributedEventBus = new DistributedEventBus(
                    originalEventBus,
                    busControl,
                    publishingStrategy,
                    messageMapper);

                iocManager.IocContainer.Register(
                    Component.For<IEventBus, IDistributedEventBus>()
                             .Instance(distributedEventBus)
                             .Named("distributed-event-bus")
                             .IsDefault()
                             .LifestyleSingleton());
            }
            else
            {
                iocManager.IocContainer.Register(
                    Component.For<IEventBus>()
                             .ImplementedBy<DistributedEventBus>()
                             .DependsOn(DependencyRegistration.OnComponent(typeof(IEventBus), "event-bus"))
                             .Named("distributed-event-bus")
                             .IsDefault()
                             .LifestyleSingleton()
                );
            }

            return busControl;
        }
    }
}
