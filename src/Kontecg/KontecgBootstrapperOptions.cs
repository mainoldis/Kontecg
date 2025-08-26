using Kontecg.Dependency;
using Kontecg.PlugIns;

namespace Kontecg
{
    public class KontecgBootstrapperOptions
    {
        public KontecgBootstrapperOptions()
        {
            IocManager = Dependency.IocManager.Instance;
            PlugInSources = new PlugInSourceList();
            InterceptorOptions = new KontecgBootstrapperInterceptorOptions();
        }

        /// <summary>
        ///     Used to disable all interceptors added by Kontecg.
        /// </summary>
        public KontecgBootstrapperInterceptorOptions InterceptorOptions { get; set; }

        /// <summary>
        ///     IIocManager that is used to bootstrap the Kontecg system. If set to null, uses global
        ///     <see cref="Dependency.IocManager.Instance" />
        /// </summary>
        public IIocManager IocManager { get; set; }

        /// <summary>
        ///     List of plugin sources.
        /// </summary>
        public PlugInSourceList PlugInSources { get; }
    }

    public class KontecgBootstrapperInterceptorOptions
    {
        public bool DisableExceptionHandlingInterceptor { get; set; }

        public bool DisableValidationInterceptor { get; set; }

        public bool DisableAuditingInterceptor { get; set; }

        public bool DisableEntityHistoryInterceptor { get; set; }

        public bool DisableUnitOfWorkInterceptor { get; set; }

        public bool DisableAuthorizationInterceptor { get; set; }
    }
}
