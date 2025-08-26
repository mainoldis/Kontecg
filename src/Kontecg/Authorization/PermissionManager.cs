using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     Permission manager.
    /// </summary>
    public class PermissionManager : PermissionDefinitionContextBase, IPermissionManager, ISingletonDependency
    {
        private readonly IAuthorizationConfiguration _authorizationConfiguration;

        private readonly IIocManager _iocManager;
        private readonly IMultiCompanyConfig _multiCompany;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public PermissionManager(
            IIocManager iocManager,
            IAuthorizationConfiguration authorizationConfiguration,
            IUnitOfWorkManager unitOfWorkManager,
            IMultiCompanyConfig multiCompany)
        {
            _iocManager = iocManager;
            _authorizationConfiguration = authorizationConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _multiCompany = multiCompany;

            KontecgSession = NullKontecgSession.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public virtual Permission GetPermission(string name)
        {
            Permission permission = Permissions.GetOrDefault(name);
            if (permission == null)
            {
                throw new KontecgException("There is no permission with name: " + name);
            }

            return permission;
        }

        public virtual IReadOnlyList<Permission> GetAllPermissions(bool companyFilter = true)
        {
            using IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                _iocManager.ResolveAsDisposable<FeatureDependencyContext>();
            FeatureDependencyContext featureDependencyContextObject = featureDependencyContext.Object;
            featureDependencyContextObject.CompanyId = GetCurrentCompanyId();

            return Permissions.Values
                .WhereIf(companyFilter, p => p.MultiCompanySides.HasFlag(GetCurrentMultiCompanySide()))
                .Where(p =>
                    p.FeatureDependency == null ||
                    GetCurrentMultiCompanySide() == MultiCompanySides.Host ||
                    p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                ).ToImmutableList();
        }

        public virtual IReadOnlyList<Permission> GetAllPermissions(MultiCompanySides multiCompanySides)
        {
            using IDisposableDependencyObjectWrapper<FeatureDependencyContext> featureDependencyContext =
                _iocManager.ResolveAsDisposable<FeatureDependencyContext>();
            FeatureDependencyContext featureDependencyContextObject = featureDependencyContext.Object;
            featureDependencyContextObject.CompanyId = GetCurrentCompanyId();

            return Permissions.Values
                .Where(p => p.MultiCompanySides.HasFlag(multiCompanySides))
                .Where(p =>
                    p.FeatureDependency == null ||
                    GetCurrentMultiCompanySide() == MultiCompanySides.Host ||
                    (p.MultiCompanySides.HasFlag(MultiCompanySides.Host) &&
                     multiCompanySides.HasFlag(MultiCompanySides.Host)) ||
                    p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                ).ToImmutableList();
        }

        public virtual async Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(bool companyFilter = true)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.CompanyId = GetCurrentCompanyId();

                var permissions = Permissions.Values
                    .WhereIf(companyFilter, p => p.MultiCompanySides.HasFlag(GetCurrentMultiCompanySide()))
                    .ToList();

                var result = await FilterSatisfiedPermissionsAsync(
                    featureDependencyContextObject,
                    permissions,
                    p => p.FeatureDependency == null || GetCurrentMultiCompanySide() == MultiCompanySides.Host
                );

                return result.ToImmutableList();
            }
        }

        public virtual void Initialize()
        {
            foreach (Type providerType in _authorizationConfiguration.Providers)
            {
                using IDisposableDependencyObjectWrapper<AuthorizationProvider> provider =
                    _iocManager.ResolveAsDisposable<AuthorizationProvider>(providerType);
                provider.Object.SetPermissions(this);
            }

            Permissions.AddAllPermissions();
        }

        private MultiCompanySides GetCurrentMultiCompanySide()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _multiCompany.IsEnabled && !_unitOfWorkManager.Current.GetCompanyId().HasValue
                    ? MultiCompanySides.Host
                    : MultiCompanySides.Company;
            }

            return KontecgSession.MultiCompanySide;
        }

        private int? GetCurrentCompanyId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetCompanyId();
            }

            return KontecgSession.CompanyId;
        }

        public async Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(MultiCompanySides multiCompanySides)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.CompanyId = GetCurrentCompanyId();

                var permissions = Permissions.Values
                    .Where(p => p.MultiCompanySides.HasFlag(multiCompanySides))
                    .ToList();

                var result = await FilterSatisfiedPermissionsAsync(
                    featureDependencyContextObject,
                    permissions,
                    p =>
                        p.FeatureDependency == null ||
                        GetCurrentMultiCompanySide() == MultiCompanySides.Host ||
                        (p.MultiCompanySides.HasFlag(MultiCompanySides.Host) &&
                         multiCompanySides.HasFlag(MultiCompanySides.Host))
                );

                return result.ToImmutableList();
            }
        }

        private async Task<IList<Permission>> FilterSatisfiedPermissionsAsync(
            FeatureDependencyContext featureDependencyContextObject,
            IList<Permission> unfilteredPermissions,
            Func<Permission, bool> filter)
        {
            var filteredPermissions = new List<Permission>();

            foreach (var permission in unfilteredPermissions)
            {
                if (!filter.Invoke(permission) &&
                    !await permission.FeatureDependency.IsSatisfiedAsync(featureDependencyContextObject))
                {
                    continue;
                }

                filteredPermissions.Add(permission);
            }

            return filteredPermissions;
        }
    }
}
