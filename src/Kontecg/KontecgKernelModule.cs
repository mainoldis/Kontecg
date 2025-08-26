using System;
using System.IO;
using System.Linq.Expressions;
using Castle.MicroKernel.Registration;
using Kontecg.Application.Features;
using Kontecg.Application.Navigation;
using Kontecg.Application.Services;
using Kontecg.Auditing;
using Kontecg.Authorization;
using Kontecg.BackgroundJobs;
using Kontecg.CachedUniqueKeys;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.DynamicEntityProperties;
using Kontecg.EntityHistory;
using Kontecg.Events.Bus;
using Kontecg.ExceptionHandling;
using Kontecg.Localization;
using Kontecg.Localization.Dictionaries;
using Kontecg.Localization.Dictionaries.Xml;
using Kontecg.Modules;
using Kontecg.MultiCompany;
using Kontecg.Net.Http;
using Kontecg.Net.Mail;
using Kontecg.Notifications;
using Kontecg.RealTime;
using Kontecg.Reflection.Extensions;
using Kontecg.Runtime;
using Kontecg.Runtime.Caching;
using Kontecg.Runtime.Remoting;
using Kontecg.Runtime.Validation.Interception;
using Kontecg.Threading;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Timing;
using Kontecg.Updates;
using Kontecg.Webhooks;
using Velopack.Sources;

namespace Kontecg
{
    /// <summary>
    /// Core kernel module of the Kontecg framework that initializes and configures all essential system components.
    /// This module is automatically loaded first and provides the foundation for all other modules.
    /// </summary>
    /// <remarks>
    /// The KontecgKernelModule is responsible for:
    /// <list type="bullet">
    /// <item><description>Registering core services and dependencies</description></item>
    /// <item><description>Configuring auditing, authorization, and validation interceptors</description></item>
    /// <item><description>Setting up unit of work filters and audit field configurations</description></item>
    /// <item><description>Initializing localization, caching, and background job systems</description></item>
    /// <item><description>Configuring setting providers and update sources</description></item>
    /// </list>
    /// This module implements the module lifecycle methods (PreInitialize, Initialize, PostInitialize, Shutdown)
    /// to ensure proper system startup and shutdown procedures.
    /// </remarks>
    public sealed class KontecgKernelModule : KontecgModule
    {
        /// <summary>
        /// Performs pre-initialization tasks including service registration, configuration setup,
        /// and system component initialization.
        /// </summary>
        /// <remarks>
        /// This method is called before the main initialization phase and sets up:
        /// <list type="bullet">
        /// <item><description>Conventional registrars for automatic service discovery</description></item>
        /// <item><description>Scoped IoC resolver and ambient scope providers</description></item>
        /// <item><description>Auditing selectors for application services</description></item>
        /// <item><description>Localization sources with XML dictionary providers</description></item>
        /// <item><description>Setting providers for various system components</description></item>
        /// <item><description>Unit of work filters for soft delete and company data</description></item>
        /// <item><description>Cache configurations with appropriate expiration times</description></item>
        /// <item><description>Method parameter validators for validation framework</description></item>
        /// <item><description>Update sources for system updates</description></item>
        /// </list>
        /// </remarks>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            IocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>),
                DependencyLifeStyle.Transient);

            AddAuditingSelectors();
            AddLocalizationSources();
            AddSettingProviders();
            AddUnitOfWorkFilters();
            AddUnitOfWorkAuditFieldConfiguration();
            ConfigureCaches();
            AddIgnoredTypes();
            AddMethodParameterValidators();
            AddUpdateSources();
        }

        /// <summary>
        /// Performs the main initialization phase including service replacement actions,
        /// event bus installation, and interceptor registration.
        /// </summary>
        /// <remarks>
        /// This method executes after PreInitialize and handles:
        /// <list type="bullet">
        /// <item><description>Service replacement actions from configuration</description></item>
        /// <item><description>Event bus installer registration</description></item>
        /// <item><description>Online client management services</description></item>
        /// <item><description>Background job event triggers</description></item>
        /// <item><description>Assembly convention registration</description></item>
        /// <item><description>Interceptor registration for cross-cutting concerns</description></item>
        /// </list>
        /// </remarks>
        public override void Initialize()
        {
            foreach (Action replaceAction in ((KontecgStartupConfiguration) Configuration).ServiceReplaceActions.Values)
            {
                replaceAction();
            }

            IocManager.IocContainer.Install(new EventBusInstaller(IocManager));

            IocManager.Register(typeof(IOnlineClientManager), typeof(OnlineClientManager));
            IocManager.Register(typeof(IOnlineClientStore), typeof(InMemoryOnlineClientStore));

            IocManager.Register(typeof(EventTriggerAsyncBackgroundJob<>), DependencyLifeStyle.Transient);

            IocManager.RegisterAssemblyByConvention(typeof(KontecgKernelModule).GetAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });

            RegisterInterceptors();
        }

        /// <summary>
        /// Performs post-initialization tasks including manager initialization and background worker startup.
        /// </summary>
        /// <remarks>
        /// This method is called after Initialize and handles:
        /// <list type="bullet">
        /// <item><description>Registration of missing components with fallback implementations</description></item>
        /// <item><description>Initialization of all system managers (Settings, Features, Permissions, etc.)</description></item>
        /// <item><description>Background worker manager startup if job execution is enabled</description></item>
        /// <item><description>Background job manager registration</description></item>
        /// </list>
        /// </remarks>
        public override void PostInitialize()
        {
            RegisterMissingComponents();

            IocManager.Resolve<SettingDefinitionManager>().Initialize();
            IocManager.Resolve<FeatureManager>().Initialize();
            IocManager.Resolve<PermissionManager>().Initialize();
            IocManager.Resolve<LocalizationManager>().Initialize();
            IocManager.Resolve<KontecgUpdateManager>().Initialize();
            IocManager.Resolve<NotificationDefinitionManager>().Initialize();
            IocManager.Resolve<NavigationManager>().Initialize();
            IocManager.Resolve<WebhookDefinitionManager>().Initialize();
            IocManager.Resolve<DynamicEntityPropertyDefinitionManager>().Initialize();

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IBackgroundWorkerManager workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Start();
                workerManager.Add(IocManager.Resolve<IBackgroundJobManager>());
            }
        }

        /// <summary>
        /// Performs shutdown procedures including background worker cleanup.
        /// </summary>
        /// <remarks>
        /// This method is called during application shutdown and ensures:
        /// <list type="bullet">
        /// <item><description>Proper shutdown of background worker manager if job execution is enabled</description></item>
        /// <item><description>Cleanup of background job processing</description></item>
        /// </list>
        /// </remarks>
        public override void Shutdown()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.Resolve<IBackgroundWorkerManager>().StopAndWaitToStop();
            }
        }

        /// <summary>
        /// Registers all cross-cutting concern interceptors for the application.
        /// </summary>
        /// <remarks>
        /// Registers interceptors for:
        /// <list type="bullet">
        /// <item><description>Unit of Work management</description></item>
        /// <item><description>Auditing operations</description></item>
        /// <item><description>Authorization checks</description></item>
        /// <item><description>Validation processing</description></item>
        /// <item><description>Entity history tracking</description></item>
        /// <item><description>Exception handling</description></item>
        /// </list>
        /// All interceptors are registered with transient lifetime for proper scoping.
        /// </remarks>
        private void RegisterInterceptors()
        {
            IocManager.Register(typeof(KontecgAsyncDeterminationInterceptor<UnitOfWorkInterceptor>),
                DependencyLifeStyle.Transient);
            IocManager.Register(typeof(KontecgAsyncDeterminationInterceptor<AuditingInterceptor>),
                DependencyLifeStyle.Transient);
            IocManager.Register(typeof(KontecgAsyncDeterminationInterceptor<AuthorizationInterceptor>),
                DependencyLifeStyle.Transient);
            IocManager.Register(typeof(KontecgAsyncDeterminationInterceptor<ValidationInterceptor>),
                DependencyLifeStyle.Transient);
            IocManager.Register(typeof(KontecgAsyncDeterminationInterceptor<EntityHistoryInterceptor>),
                DependencyLifeStyle.Transient);
            IocManager.Register(typeof(KontecgAsyncDeterminationInterceptor<ExceptionHandlerInterceptor>),
                DependencyLifeStyle.Transient);
        }

        /// <summary>
        /// Registers unit of work filters for data filtering operations.
        /// </summary>
        /// <remarks>
        /// Registers the following filters:
        /// <list type="bullet">
        /// <item><description>SoftDelete filter for handling soft-deleted entities</description></item>
        /// <item><description>MustHaveCompany filter for multi-company data isolation</description></item>
        /// <item><description>MayHaveCompany filter for optional company association</description></item>
        /// </list>
        /// All filters are enabled by default.
        /// </remarks>
        private void AddUnitOfWorkFilters()
        {
            Configuration.UnitOfWork.RegisterFilter(KontecgDataFilters.SoftDelete, true);
            Configuration.UnitOfWork.RegisterFilter(KontecgDataFilters.MustHaveCompany, true);
            Configuration.UnitOfWork.RegisterFilter(KontecgDataFilters.MayHaveCompany, true);
        }

        /// <summary>
        /// Configures audit field tracking for unit of work operations.
        /// </summary>
        /// <remarks>
        /// Registers audit field configurations for:
        /// <list type="bullet">
        /// <item><description>CreatorUserId - tracks who created the entity</description></item>
        /// <item><description>LastModifierUserId - tracks who last modified the entity</description></item>
        /// <item><description>LastModificationTime - tracks when the entity was last modified</description></item>
        /// <item><description>DeleterUserId - tracks who deleted the entity (for soft deletes)</description></item>
        /// <item><description>DeletionTime - tracks when the entity was deleted (for soft deletes)</description></item>
        /// </list>
        /// All audit field configurations are enabled by default.
        /// </remarks>
        private void AddUnitOfWorkAuditFieldConfiguration()
        {
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(KontecgAuditFields.CreatorUserId, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(KontecgAuditFields.LastModifierUserId, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(KontecgAuditFields.LastModificationTime, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(KontecgAuditFields.DeleterUserId, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(KontecgAuditFields.DeletionTime, true);
        }

        /// <summary>
        /// Adds setting providers for various system components.
        /// </summary>
        /// <remarks>
        /// Registers setting providers for:
        /// <list type="bullet">
        /// <item><description>Localization settings</description></item>
        /// <item><description>Email configuration settings</description></item>
        /// <item><description>HTTP client settings</description></item>
        /// <item><description>Notification settings</description></item>
        /// <item><description>Timing and scheduling settings</description></item>
        /// </list>
        /// These providers supply default configuration values for their respective components.
        /// </remarks>
        private void AddSettingProviders()
        {
            Configuration.Settings.Providers.Add<LocalizationSettingProvider>();
            Configuration.Settings.Providers.Add<EmailSettingProvider>();
            Configuration.Settings.Providers.Add<HttpSettingProvider>();
            Configuration.Settings.Providers.Add<NotificationSettingProvider>();
            Configuration.Settings.Providers.Add<TimingSettingProvider>();
        }

        /// <summary>
        /// Configures auditing selectors for application services.
        /// </summary>
        /// <remarks>
        /// Adds a named type selector that automatically audits all types implementing
        /// <see cref="IApplicationService"/>. This ensures that all application service
        /// method calls are automatically logged for audit purposes.
        /// </remarks>
        private void AddAuditingSelectors()
        {
            Configuration.Auditing.Selectors.Add(
                new NamedTypeSelector(
                    "Kontecg.ApplicationServices",
                    type => typeof(IApplicationService).IsAssignableFrom(type)
                )
            );
        }

        /// <summary>
        /// Configures localization sources with XML dictionary providers.
        /// </summary>
        /// <remarks>
        /// Sets up the default localization source using XML embedded files located in
        /// the "Kontecg.Localization.Sources.KontecgXmlSource" assembly resource path.
        /// This provides the foundation for multi-language support in the application.
        /// </remarks>
        private void AddLocalizationSources()
        {
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    KontecgConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(KontecgKernelModule).GetAssembly(), "Kontecg.Localization.Sources.KontecgXmlSource"
                    )));
        }

        /// <summary>
        /// Configures cache settings with appropriate expiration times for different cache types.
        /// </summary>
        /// <remarks>
        /// Configures the following caches:
        /// <list type="bullet">
        /// <item><description>ApplicationSettings - 8 hours sliding expiration</description></item>
        /// <item><description>CompanySettings - 60 minutes sliding expiration</description></item>
        /// <item><description>UserSettings - 20 minutes sliding expiration</description></item>
        /// </list>
        /// These expiration times are optimized for the typical usage patterns of each cache type.
        /// </remarks>
        private void ConfigureCaches()
        {
            Configuration.Caching.Configure(KontecgCacheNames.ApplicationSettings,
                cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8); });

            Configuration.Caching.Configure(KontecgCacheNames.CompanySettings,
                cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(60); });

            Configuration.Caching.Configure(KontecgCacheNames.UserSettings,
                cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(20); });
        }

        /// <summary>
        /// Adds types to be ignored by auditing and validation systems.
        /// </summary>
        /// <remarks>
        /// Configures the following ignored types:
        /// <list type="bullet">
        /// <item><description>Common ignored types (Stream, Expression) for both auditing and validation</description></item>
        /// <item><description>Validation-only ignored types (Type) for validation system</description></item>
        /// </list>
        /// These types are excluded from auditing and validation to avoid performance issues
        /// and reduce noise in audit logs.
        /// </remarks>
        private void AddIgnoredTypes()
        {
            Type[] commonIgnoredTypes = new[]
            {
                typeof(Stream),
                typeof(Expression)
            };

            foreach (Type ignoredType in commonIgnoredTypes)
            {
                Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }

            Type[] validationIgnoredTypes = new[] {typeof(Type)};
            foreach (Type ignoredType in validationIgnoredTypes)
            {
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }

        /// <summary>
        /// Adds method parameter validators for the validation framework.
        /// </summary>
        /// <remarks>
        /// Registers the following validators:
        /// <list type="bullet">
        /// <item><description>DataAnnotationsValidator - for Data Annotations validation</description></item>
        /// <item><description>ValidatableObjectValidator - for IValidatableObject implementations</description></item>
        /// <item><description>CustomValidator - for custom validation logic</description></item>
        /// </list>
        /// These validators provide comprehensive validation capabilities for method parameters.
        /// </remarks>
        private void AddMethodParameterValidators()
        {
            Configuration.Validation.Validators.Add<DataAnnotationsValidator>();
            Configuration.Validation.Validators.Add<ValidatableObjectValidator>();
            Configuration.Validation.Validators.Add<CustomValidator>();
        }

        /// <summary>
        /// Adds update sources for system updates.
        /// </summary>
        /// <remarks>
        /// Configures the NuGet API as the primary update source for system updates.
        /// This allows the application to check for and download updates from the
        /// official NuGet package repository.
        /// </remarks>
        private void AddUpdateSources()
        {
            Configuration.Updates.Sources.Add(new KontecgLocalSource(new Uri("https://api.nuget.org/v3/index.json")));
        }

        /// <summary>
        /// Registers missing components with fallback implementations.
        /// </summary>
        /// <remarks>
        /// Registers fallback implementations for components that may not be explicitly
        /// registered by other modules. This ensures the system has all required services
        /// available, even if they are not explicitly configured. Includes:
        /// <list type="bullet">
        /// <item><description>Guid generator with UUID implementation</description></item>
        /// <item><description>Unit of work with null implementation</description></item>
        /// <item><description>Auditing store with simple log implementation</description></item>
        /// <item><description>Permission checker with null implementation</description></item>
        /// <item><description>Notification store with null implementation</description></item>
        /// <item><description>Unit of work filter executor with null implementation</description></item>
        /// <item><description>Client info provider with null implementation</description></item>
        /// <item><description>Company store and resolver cache with null implementations</description></item>
        /// <item><description>Entity history store with null implementation</description></item>
        /// <item><description>Cached unique key per user with default implementation</description></item>
        /// <item><description>HTTP client provider with preconfigured implementation</description></item>
        /// <item><description>File downloader with default implementation</description></item>
        /// <item><description>Background job store based on job execution configuration</description></item>
        /// </list>
        /// </remarks>
        private void RegisterMissingComponents()
        {
            if (!IocManager.IsRegistered<IGuidGenerator>())
            {
                IocManager.IocContainer.Register(
                    Component
                        .For<IGuidGenerator, UuidGenerator>()
                        .Instance(UuidGenerator.Instance)
                );
            }

            IocManager.RegisterIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>();
            IocManager.RegisterIfNot<IPermissionChecker, NullPermissionChecker>();
            IocManager.RegisterIfNot<INotificationStore, NullNotificationStore>();
            IocManager.RegisterIfNot<IUnitOfWorkFilterExecuter, NullUnitOfWorkFilterExecuter>();
            IocManager.RegisterIfNot<IClientInfoProvider, NullClientInfoProvider>();
            IocManager.RegisterIfNot<ICompanyStore, NullCompanyStore>();
            IocManager.RegisterIfNot<ICompanyResolverCache, NullCompanyResolverCache>();
            IocManager.RegisterIfNot<IEntityHistoryStore, NullEntityHistoryStore>();
            IocManager.RegisterIfNot<ICachedUniqueKeyPerUser, CachedUniqueKeyPerUser>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IPreconfiguredHttpClientProvider, PreconfiguredHttpClientProvider>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IFileDownloader, FileDownLoader>(DependencyLifeStyle.Transient);

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.RegisterIfNot<IBackgroundJobStore, InMemoryBackgroundJobStore>();
            }
            else
            {
                IocManager.RegisterIfNot<IBackgroundJobStore, NullBackgroundJobStore>();
            }
        }
    }
}
