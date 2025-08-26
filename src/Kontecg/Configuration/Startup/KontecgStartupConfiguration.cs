using System;
using System.Collections.Generic;
using Kontecg.Application.Features;
using Kontecg.Auditing;
using Kontecg.BackgroundJobs;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.DynamicEntityProperties;
using Kontecg.EntityHistory;
using Kontecg.Events.Bus;
using Kontecg.ExceptionHandling;
using Kontecg.Notifications;
using Kontecg.Resources.Embedded;
using Kontecg.Runtime.Caching.Configuration;
using Kontecg.Updates;
using Kontecg.Webhooks;

namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     This class is used to configure Kontecg and modules on startup.
    /// </summary>
    internal class KontecgStartupConfiguration : DictionaryBasedConfig, IKontecgStartupConfiguration
    {
        /// <summary>
        ///     Private constructor for singleton pattern.
        /// </summary>
        public KontecgStartupConfiguration(IIocManager iocManager)
        {
            IocManager = iocManager;
        }

        public Dictionary<Type, Action> ServiceReplaceActions { get; private set; }

        /// <summary>
        ///     Reference to the IocManager.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        ///     Used to set localization configuration.
        /// </summary>
        public ILocalizationConfiguration Localization { get; private set; }

        /// <summary>
        ///     Used to configure authorization.
        /// </summary>
        public IAuthorizationConfiguration Authorization { get; private set; }

        /// <summary>
        ///     Used to configure validation.
        /// </summary>
        public IValidationConfiguration Validation { get; private set; }

        /// <summary>
        ///     Used to configure settings.
        /// </summary>
        public ISettingsConfiguration Settings { get; private set; }

        /// <summary>
        ///     Gets/sets default connection string used by ORM module.
        ///     It can be name of a connection string in application's config file or can be full connection string.
        /// </summary>
        public string DefaultNameOrConnectionString { get; set; }

        /// <summary>
        ///     Used to configure modules.
        ///     Modules can write extension methods to <see cref="ModuleConfigurations" /> to add module specific configurations.
        /// </summary>
        public IModuleConfigurations Modules { get; private set; }

        /// <summary>
        ///     Used to configure unit of work defaults.
        /// </summary>
        public IUnitOfWorkDefaultOptions UnitOfWork { get; private set; }

        /// <summary>
        ///     Used to configure features.
        /// </summary>
        public IFeatureConfiguration Features { get; private set; }

        /// <summary>
        ///     Used to configure Updates.
        /// </summary>
        public IUpdateConfiguration Updates { get; private set; }

        /// <summary>
        ///     Used to configure background job system.
        /// </summary>
        public IBackgroundJobConfiguration BackgroundJobs { get; private set; }

        /// <summary>
        ///     Used to configure notification system.
        /// </summary>
        public INotificationConfiguration Notifications { get; private set; }

        /// <inheritdoc />
        public IExceptionHandlingConfiguration ExceptionHandling { get; private set; }

        /// <summary>
        ///     Used to configure navigation.
        /// </summary>
        public INavigationConfiguration Navigation { get; private set; }

        /// <summary>
        ///     Used to configure <see cref="IEventBus" />.
        /// </summary>
        public IEventBusConfiguration EventBus { get; private set; }

        /// <summary>
        ///     Used to configure auditing.
        /// </summary>
        public IAuditingConfiguration Auditing { get; private set; }

        public ICachingConfiguration Caching { get; private set; }

        /// <summary>
        ///     Used to configure multi-company.
        /// </summary>
        public IMultiCompanyConfig MultiCompany { get; private set; }

        public IEmbeddedResourcesConfiguration EmbeddedResources { get; private set; }

        public IEntityHistoryConfiguration EntityHistory { get; private set; }

        public IWebhooksConfiguration Webhooks { get; private set; }

        public IWorkflowsConfiguration Workflows { get; private set; }

        public IDynamicEntityPropertyConfiguration DynamicEntityProperties { get; private set; }

        public IList<ICustomConfigProvider> CustomConfigProviders { get; private set; }

        public Dictionary<string, object> GetCustomConfig()
        {
            Dictionary<string, object> mergedConfig = new Dictionary<string, object>();

            using IScopedIocResolver scope = IocManager.CreateScope();
            foreach (ICustomConfigProvider provider in CustomConfigProviders)
            {
                Dictionary<string, object> config = provider.GetConfig(new CustomConfigProviderContext(scope));
                foreach (KeyValuePair<string, object> keyValue in config)
                {
                    mergedConfig[keyValue.Key] = keyValue.Value;
                }
            }

            return mergedConfig;
        }

        public void ReplaceService(Type type, Action replaceAction)
        {
            ServiceReplaceActions[type] = replaceAction;
        }

        public T Get<T>()
        {
            return GetOrCreate(typeof(T).FullName, IocManager.Resolve<T>);
        }

        public void Initialize()
        {
            Localization = IocManager.Resolve<ILocalizationConfiguration>();
            Modules = IocManager.Resolve<IModuleConfigurations>();
            Features = IocManager.Resolve<IFeatureConfiguration>();
            Navigation = IocManager.Resolve<INavigationConfiguration>();
            Authorization = IocManager.Resolve<IAuthorizationConfiguration>();
            Validation = IocManager.Resolve<IValidationConfiguration>();
            Settings = IocManager.Resolve<ISettingsConfiguration>();
            UnitOfWork = IocManager.Resolve<IUnitOfWorkDefaultOptions>();
            EventBus = IocManager.Resolve<IEventBusConfiguration>();
            MultiCompany = IocManager.Resolve<IMultiCompanyConfig>();
            Auditing = IocManager.Resolve<IAuditingConfiguration>();
            Caching = IocManager.Resolve<ICachingConfiguration>();
            BackgroundJobs = IocManager.Resolve<IBackgroundJobConfiguration>();
            Updates = IocManager.Resolve<IUpdateConfiguration>();
            Notifications = IocManager.Resolve<INotificationConfiguration>();
            ExceptionHandling = IocManager.Resolve<IExceptionHandlingConfiguration>();
            EmbeddedResources = IocManager.Resolve<IEmbeddedResourcesConfiguration>();
            EntityHistory = IocManager.Resolve<IEntityHistoryConfiguration>();
            Webhooks = IocManager.Resolve<IWebhooksConfiguration>();
            Workflows = IocManager.Resolve<IWorkflowsConfiguration>();
            DynamicEntityProperties = IocManager.Resolve<IDynamicEntityPropertyConfiguration>();

            CustomConfigProviders = new List<ICustomConfigProvider>();
            ServiceReplaceActions = new Dictionary<Type, Action>();
        }
    }
}
