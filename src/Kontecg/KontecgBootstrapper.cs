using System;
using System.Reflection;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using JetBrains.Annotations;
using Kontecg.Auditing;
using Kontecg.Authorization;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Dependency.Installers;
using Kontecg.Domain.Uow;
using Kontecg.EntityHistory;
using Kontecg.ExceptionHandling;
using Kontecg.Modules;
using Kontecg.PlugIns;
using Kontecg.Runtime.Validation.Interception;

namespace Kontecg
{
    /// <summary>
    ///     This is the main class that is responsible to start entire Kontecg system.
    ///     Prepares dependency injection and registers core components needed for startup.
    ///     It must be instantiated and initialized (see <see cref="Initialize" />) first in an application.
    /// </summary>
    public class KontecgBootstrapper : IDisposable
    {
        private ILogger _logger;

        private KontecgModuleManager _moduleManager;

        /// <summary>
        ///     Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        /// <summary>
        ///     Creates a new <see cref="KontecgBootstrapper" /> instance.
        /// </summary>
        /// <param name="startupModule">
        ///     Startup module of the application which depends on other used modules. Should be derived
        ///     from <see cref="KontecgModule" />.
        /// </param>
        /// <param name="optionsAction">An action to set options</param>
        private KontecgBootstrapper([NotNull] Type startupModule,
            [CanBeNull] Action<KontecgBootstrapperOptions> optionsAction = null)
        {
            Check.NotNull(startupModule, nameof(startupModule));

            KontecgBootstrapperOptions options = new KontecgBootstrapperOptions();
            optionsAction?.Invoke(options);

            if (!typeof(KontecgModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(KontecgModule)}.");
            }

            StartupModule = startupModule;

            IocManager = options.IocManager;
            PlugInSources = options.PlugInSources;

            _logger = NullLogger.Instance;

            AddInterceptorRegistrars(options.InterceptorOptions);
        }

        /// <summary>
        ///     Get the startup module of the application which depends on other used modules.
        /// </summary>
        public Type StartupModule { get; }

        /// <summary>
        ///     A list of plug in folders.
        /// </summary>
        public PlugInSourceList PlugInSources { get; }

        /// <summary>
        ///     Gets IIocManager object used by this class.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        ///     Disposes the Kontecg system.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            _moduleManager?.ShutdownModules();
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgBootstrapper" /> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">
        ///     Startup module of the application which depends on other used modules. Should be
        ///     derived from <see cref="KontecgModule" />.
        /// </typeparam>
        /// <param name="optionsAction">An action to set options</param>
        public static KontecgBootstrapper Create<TStartupModule>(
            [CanBeNull] Action<KontecgBootstrapperOptions> optionsAction = null)
            where TStartupModule : KontecgModule
        {
            return new KontecgBootstrapper(typeof(TStartupModule), optionsAction);
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgBootstrapper" /> instance.
        /// </summary>
        /// <param name="startupModule">
        ///     Startup module of the application which depends on other used modules. Should be derived
        ///     from <see cref="KontecgModule" />.
        /// </param>
        /// <param name="optionsAction">An action to set options</param>
        public static KontecgBootstrapper Create([NotNull] Type startupModule,
            [CanBeNull] Action<KontecgBootstrapperOptions> optionsAction = null)
        {
            return new KontecgBootstrapper(startupModule, optionsAction);
        }

        /// <summary>
        ///     Initializes the Kontecg system.
        /// </summary>
        public virtual void Initialize()
        {
            ResolveLogger();

            try
            {
                RegisterBootstrapper();
                IocManager.IocContainer.Install(new KontecgInstaller());

                IocManager.Resolve<KontecgPlugInManager>().PlugInSources.AddRange(PlugInSources);
                IocManager.Resolve<KontecgStartupConfiguration>().Initialize();

                _moduleManager = IocManager.Resolve<KontecgModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.ToString(), ex);
                throw;
            }
        }

        private void AddInterceptorRegistrars(
            KontecgBootstrapperInterceptorOptions options)
        {
            if (!options.DisableValidationInterceptor)
            {
                ValidationInterceptorRegistrar.Initialize(IocManager);
            }

            if (!options.DisableAuditingInterceptor)
            {
                AuditingInterceptorRegistrar.Initialize(IocManager);
            }

            if (!options.DisableEntityHistoryInterceptor)
            {
                EntityHistoryInterceptorRegistrar.Initialize(IocManager);
            }

            if (!options.DisableUnitOfWorkInterceptor)
            {
                UnitOfWorkRegistrar.Initialize(IocManager);
            }

            if (!options.DisableAuthorizationInterceptor)
            {
                AuthorizationInterceptorRegistrar.Initialize(IocManager);
            }

            if (!options.DisableExceptionHandlingInterceptor)
            {
                ExceptionHandlerInterceptorRegistrar.Initialize(IocManager);
            }
        }

        private void ResolveLogger()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                _logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(KontecgBootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<KontecgBootstrapper>())
            {
                IocManager.IocContainer.Register(
                    Component.For<KontecgBootstrapper>().Instance(this)
                );
            }
        }
    }
}
