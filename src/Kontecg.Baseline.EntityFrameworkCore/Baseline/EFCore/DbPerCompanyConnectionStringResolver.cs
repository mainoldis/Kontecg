using System.Threading.Tasks;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;

namespace Kontecg.Baseline.EFCore
{
    /// <summary>
    ///     Implements <see cref="IDbPerCompanyConnectionStringResolver" /> to dynamically resolve
    ///     connection string for a multi company application.
    /// </summary>
    public class DbPerCompanyConnectionStringResolver : DefaultConnectionStringResolver,
        IDbPerCompanyConnectionStringResolver
    {
        private readonly ICompanyCache _companyCache;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbPerCompanyConnectionStringResolver" /> class.
        /// </summary>
        public DbPerCompanyConnectionStringResolver(
            IKontecgStartupConfiguration configuration,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            ICompanyCache companyCache)
            : base(configuration)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _companyCache = companyCache;

            KontecgSession = NullKontecgSession.Instance;
        }

        /// <summary>
        ///     Reference to the session.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        public override string GetNameOrConnectionString(ConnectionStringResolveArgs args)
        {
            if (args.MultiCompanySide == MultiCompanySides.Host)
            {
                return GetNameOrConnectionString(new DbPerCompanyConnectionStringResolveArgs(null, args));
            }

            return GetNameOrConnectionString(new DbPerCompanyConnectionStringResolveArgs(GetCurrentCompanyId(), args));
        }

        public virtual string GetNameOrConnectionString(DbPerCompanyConnectionStringResolveArgs args)
        {
            if (args.CompanyId == null)
                //Requested for host
            {
                return base.GetNameOrConnectionString(args);
            }

            CompanyCacheItem companyCacheItem = _companyCache.Get(args.CompanyId.Value);
            if (companyCacheItem.ConnectionString.IsNullOrEmpty())
                //Company has not dedicated database
            {
                return base.GetNameOrConnectionString(args);
            }

            return companyCacheItem.ConnectionString;
        }

        public override async Task<string> GetNameOrConnectionStringAsync(ConnectionStringResolveArgs args)
        {
            if (args.MultiCompanySide == MultiCompanySides.Host)
            {
                return await GetNameOrConnectionStringAsync(new DbPerCompanyConnectionStringResolveArgs(null, args));
            }

            return await GetNameOrConnectionStringAsync(
                new DbPerCompanyConnectionStringResolveArgs(GetCurrentCompanyId(), args));
        }

        public virtual async Task<string> GetNameOrConnectionStringAsync(DbPerCompanyConnectionStringResolveArgs args)
        {
            if (args.CompanyId == null)
                //Requested for host
            {
                return await base.GetNameOrConnectionStringAsync(args);
            }

            CompanyCacheItem companyCacheItem = await _companyCache.GetAsync(args.CompanyId.Value);
            if (companyCacheItem.ConnectionString.IsNullOrEmpty())
                //Company has not dedicated database
            {
                return await base.GetNameOrConnectionStringAsync(args);
            }

            return companyCacheItem.ConnectionString;
        }

        protected virtual int? GetCurrentCompanyId()
        {
            return _currentUnitOfWorkProvider.Current != null
                ? _currentUnitOfWorkProvider.Current.GetCompanyId()
                : KontecgSession.CompanyId;
        }
    }
}
