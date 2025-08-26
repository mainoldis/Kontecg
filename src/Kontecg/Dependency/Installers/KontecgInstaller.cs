using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Kontecg.Application.Features;
using Kontecg.Auditing;
using Kontecg.BackgroundJobs;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Uow;
using Kontecg.DynamicEntityProperties;
using Kontecg.EntityHistory;
using Kontecg.ExceptionHandling;
using Kontecg.Localization;
using Kontecg.Modules;
using Kontecg.Notifications;
using Kontecg.PlugIns;
using Kontecg.Reflection;
using Kontecg.Resources.Embedded;
using Kontecg.Runtime.Caching.Configuration;
using Kontecg.Runtime.Validation;
using Kontecg.Updates;
using Kontecg.Web.Markdown;
using Kontecg.Webhooks;

namespace Kontecg.Dependency.Installers
{
    internal class KontecgInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IKontecgExceptionHandlingDefaultOptions, KontecgExceptionHandlingDefaultOptions>()
                    .ImplementedBy<KontecgExceptionHandlingDefaultOptions>().LifestyleSingleton(),
                Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>()
                    .ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
                Component.For<IKontecgValidationDefaultOptions, KontecgValidationDefaultOptions>()
                    .ImplementedBy<KontecgValidationDefaultOptions>().LifestyleSingleton(),
                Component.For<IKontecgAuditingDefaultOptions, KontecgAuditingDefaultOptions>()
                    .ImplementedBy<KontecgAuditingDefaultOptions>().LifestyleSingleton(),
                Component.For<IExceptionHandlingConfiguration, ExceptionHandlingConfiguration>()
                    .ImplementedBy<ExceptionHandlingConfiguration>().LifestyleSingleton(),
                Component.For<INavigationConfiguration, NavigationConfiguration>()
                    .ImplementedBy<NavigationConfiguration>().LifestyleSingleton(),
                Component.For<ILocalizationConfiguration, LocalizationConfiguration>()
                    .ImplementedBy<LocalizationConfiguration>().LifestyleSingleton(),
                Component.For<IUpdateConfiguration, UpdateConfiguration>()
                    .ImplementedBy<UpdateConfiguration>().LifestyleSingleton(),
                Component.For<IAuthorizationConfiguration, AuthorizationConfiguration>()
                    .ImplementedBy<AuthorizationConfiguration>().LifestyleSingleton(),
                Component.For<IValidationConfiguration, ValidationConfiguration>()
                    .ImplementedBy<ValidationConfiguration>().LifestyleSingleton(),
                Component.For<IFeatureConfiguration, FeatureConfiguration>().ImplementedBy<FeatureConfiguration>()
                    .LifestyleSingleton(),
                Component.For<ISettingsConfiguration, SettingsConfiguration>().ImplementedBy<SettingsConfiguration>()
                    .LifestyleSingleton(),
                Component.For<IModuleConfigurations, ModuleConfigurations>().ImplementedBy<ModuleConfigurations>()
                    .LifestyleSingleton(),
                Component.For<IEventBusConfiguration, EventBusConfiguration>().ImplementedBy<EventBusConfiguration>()
                    .LifestyleSingleton(),
                Component.For<IMultiCompanyConfig, MultiCompanyConfig>().ImplementedBy<MultiCompanyConfig>()
                    .LifestyleSingleton(),
                Component.For<ICachingConfiguration, CachingConfiguration>().ImplementedBy<CachingConfiguration>()
                    .LifestyleSingleton(),
                Component.For<IAuditingConfiguration, AuditingConfiguration>().ImplementedBy<AuditingConfiguration>()
                    .LifestyleSingleton(),
                Component.For<IBackgroundJobConfiguration, BackgroundJobConfiguration>()
                    .ImplementedBy<BackgroundJobConfiguration>().LifestyleSingleton(),
                Component.For<INotificationConfiguration, NotificationConfiguration>()
                    .ImplementedBy<NotificationConfiguration>().LifestyleSingleton(),
                Component.For<IEmbeddedResourcesConfiguration, EmbeddedResourcesConfiguration>()
                    .ImplementedBy<EmbeddedResourcesConfiguration>().LifestyleSingleton(),
                Component.For<IKontecgStartupConfiguration, KontecgStartupConfiguration>()
                    .ImplementedBy<KontecgStartupConfiguration>().LifestyleSingleton(),
                Component.For<IEntityHistoryConfiguration, EntityHistoryConfiguration>()
                    .ImplementedBy<EntityHistoryConfiguration>().LifestyleSingleton(),
                Component.For<ITypeFinder, TypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton(),
                Component.For<IKontecgPlugInManager, KontecgPlugInManager>().ImplementedBy<KontecgPlugInManager>()
                    .LifestyleSingleton(),
                Component.For<IKontecgModuleManager, KontecgModuleManager>().ImplementedBy<KontecgModuleManager>()
                    .LifestyleSingleton(),
                Component.For<IAssemblyFinder, KontecgAssemblyFinder>().ImplementedBy<KontecgAssemblyFinder>()
                    .LifestyleSingleton(),
                Component.For<ILocalizationManager, LocalizationManager>().ImplementedBy<LocalizationManager>()
                    .LifestyleSingleton(),
                Component.For<IUpdateManager, KontecgUpdateManager>().ImplementedBy<KontecgUpdateManager>()
                    .LifestyleSingleton(),
                Component.For<IWebhooksConfiguration, WebhooksConfiguration>().ImplementedBy<WebhooksConfiguration>()
                    .LifestyleSingleton(),
                Component.For<IWorkflowsConfiguration, WorkflowsConfiguration>().ImplementedBy<WorkflowsConfiguration>()
                    .LifestyleSingleton(),
                Component.For<IDynamicEntityPropertyDefinitionContext, DynamicEntityPropertyDefinitionContext>()
                    .ImplementedBy<DynamicEntityPropertyDefinitionContext>().LifestyleTransient(),
                Component.For<IDynamicEntityPropertyConfiguration, DynamicEntityPropertyConfiguration>()
                    .ImplementedBy<DynamicEntityPropertyConfiguration>().LifestyleSingleton(),
                Component.For<IMarkdownDefaultOptions, MarkdownDefaultOptions>().ImplementedBy<MarkdownDefaultOptions>()
                    .LifestyleSingleton()
            );
        }
    }
}
