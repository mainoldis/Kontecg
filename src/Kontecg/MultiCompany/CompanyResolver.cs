using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Runtime;

namespace Kontecg.MultiCompany
{
    public class CompanyResolver : ICompanyResolver, ITransientDependency
    {
        private const string AmbientScopeContextKey = "Kontecg.MultiCompany.CompanyResolver.Resolving";
        private readonly IAmbientScopeProvider<bool> _ambientScopeProvider;
        private readonly ICompanyResolverCache _cache;
        private readonly ICompanyStore _companyStore;
        private readonly IIocResolver _iocResolver;

        private readonly IMultiCompanyConfig _multiCompany;

        public CompanyResolver(
            IMultiCompanyConfig multiCompany,
            IIocResolver iocResolver,
            ICompanyStore companyStore,
            ICompanyResolverCache cache,
            IAmbientScopeProvider<bool> ambientScopeProvider)
        {
            _multiCompany = multiCompany;
            _iocResolver = iocResolver;
            _companyStore = companyStore;
            _cache = cache;
            _ambientScopeProvider = ambientScopeProvider;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public int? ResolveCompanyId()
        {
            if (!_multiCompany.Resolvers.Any())
            {
                return null;
            }

            if (_ambientScopeProvider.GetValue(AmbientScopeContextKey))
                //Preventing recursive call of ResolveCompanyId
            {
                return null;
            }

            using (_ambientScopeProvider.BeginScope(AmbientScopeContextKey, true))
            {
                CompanyResolverCacheItem cacheItem = _cache.Value;
                if (cacheItem != null)
                {
                    return cacheItem.CompanyId;
                }

                int? companyId = GetCompanyIdFromContributors();
                _cache.Value = new CompanyResolverCacheItem(companyId);
                return companyId;
            }
        }

        public Task<int?> ResolveCompanyIdAsync()
        {
            return Task.FromResult(ResolveCompanyId());
        }

        private int? GetCompanyIdFromContributors()
        {
            foreach (Type resolverType in _multiCompany.Resolvers)
            {
                using IDisposableDependencyObjectWrapper<ICompanyResolveContributor> resolver =
                    _iocResolver.ResolveAsDisposable<ICompanyResolveContributor>(resolverType);
                int? companyId;

                try
                {
                    companyId = resolver.Object.ResolveCompanyId();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                    continue;
                }

                if (companyId == null)
                {
                    continue;
                }

                if (_companyStore.Find(companyId.Value) == null)
                {
                    continue;
                }

                return companyId;
            }

            return null;
        }
    }
}
