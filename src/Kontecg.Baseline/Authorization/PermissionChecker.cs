using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Runtime.Session;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     Application should inherit this class to implement <see cref="IPermissionChecker" />.
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    public class PermissionChecker<TRole, TUser> : IPermissionChecker, ITransientDependency, IIocManagerAccessor
        where TRole : KontecgRole<TUser>, new()
        where TUser : KontecgUser<TUser>
    {
        private readonly KontecgUserManager<TRole, TUser> _userManager;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public PermissionChecker(KontecgUserManager<TRole, TUser> userManager)
        {
            _userManager = userManager;

            Logger = NullLogger.Instance;
            KontecgSession = NullKontecgSession.Instance;
        }

        public ILogger Logger { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        public IIocManager IocManager { get; set; }

        public virtual async Task<bool> IsGrantedAsync(string permissionName)
        {
            return KontecgSession.UserId.HasValue && await IsGrantedAsync(KontecgSession.UserId.Value, permissionName);
        }

        public virtual bool IsGranted(string permissionName)
        {
            return KontecgSession.UserId.HasValue && IsGranted(KontecgSession.UserId.Value, permissionName);
        }

        public virtual async Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (CurrentUnitOfWorkProvider?.Current == null)
                {
                    return await IsGrantedAsync(user.UserId, permissionName);
                }

                using (CurrentUnitOfWorkProvider.Current.SetCompanyId(user.CompanyId))
                {
                    return await IsGrantedAsync(user.UserId, permissionName);
                }
            });
        }

        public virtual bool IsGranted(UserIdentifier user, string permissionName)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                if (CurrentUnitOfWorkProvider?.Current == null)
                {
                    return IsGranted(user.UserId, permissionName);
                }

                using (CurrentUnitOfWorkProvider.Current.SetCompanyId(user.CompanyId))
                {
                    return IsGranted(user.UserId, permissionName);
                }
            });
        }

        public virtual async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            return await _userManager.IsGrantedAsync(userId, permissionName);
        }

        public virtual bool IsGranted(long userId, string permissionName)
        {
            return _userManager.IsGranted(userId, permissionName);
        }
    }
}
