using System;
using Kontecg.Application.Clients;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.Configuration;
using Kontecg.IdentityFramework;
using Kontecg.MultiCompany;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class KontecgBaselineServiceCollectionExtensions
    {
        public static KontecgIdentityBuilder AddKontecgIdentity<TCompany, TUser, TRole>(
            this IServiceCollection services)
            where TCompany : KontecgCompany<TUser>
            where TRole : KontecgRole<TUser>, new()
            where TUser : KontecgUser<TUser>
        {
            return services.AddKontecgIdentity<TCompany, TUser, TRole>(null);
        }

        public static KontecgIdentityBuilder AddKontecgIdentity<TCompany, TUser, TRole>(
            this IServiceCollection services,
            Action<IdentityOptions> setupAction)
            where TCompany : KontecgCompany<TUser>
            where TRole : KontecgRole<TUser>, new()
            where TUser : KontecgUser<TUser>
        {
            services.AddSingleton<IKontecgBaselineEntityTypes>(new KontecgBaselineEntityTypes
            {
                Company = typeof(TCompany),
                Role = typeof(TRole),
                User = typeof(TUser)
            });

            //KontecgCompanyManager
            services.TryAddScoped<KontecgCompanyManager<TCompany, TUser>>();

            //KontecgClientManager
            services.TryAddScoped<KontecgClientManager<TCompany, TUser>>();

            //KontecgRoleManager
            services.TryAddScoped<KontecgRoleManager<TRole, TUser>>();
            services.TryAddScoped(typeof(RoleManager<TRole>),
                provider => provider.GetService(typeof(KontecgRoleManager<TRole, TUser>)));

            //KontecgUserManager
            services.TryAddScoped<KontecgUserManager<TRole, TUser>>();
            services.TryAddScoped(typeof(UserManager<TUser>),
                provider => provider.GetService(typeof(KontecgUserManager<TRole, TUser>)));

            //KontecgLogInManager
            services.TryAddScoped<KontecgLogInManager<TCompany, TRole, TUser>>();

            //KontecgUserClaimsPrincipalFactory
            services.TryAddScoped<KontecgUserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped(typeof(UserClaimsPrincipalFactory<TUser, TRole>),
                provider => provider.GetService(typeof(KontecgUserClaimsPrincipalFactory<TUser, TRole>)));
            services.TryAddScoped(typeof(IUserClaimsPrincipalFactory<TUser>),
                provider => provider.GetService(typeof(KontecgUserClaimsPrincipalFactory<TUser, TRole>)));

            //PermissionChecker
            services.TryAddScoped<PermissionChecker<TRole, TUser>>();
            services.TryAddScoped(typeof(IPermissionChecker),
                provider => provider.GetService(typeof(PermissionChecker<TRole, TUser>)));

            //KontecgUserStore
            services.TryAddScoped<KontecgUserStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IUserStore<TUser>),
                provider => provider.GetService(typeof(KontecgUserStore<TRole, TUser>)));

            //KontecgRoleStore
            services.TryAddScoped<KontecgRoleStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IRoleStore<TRole>),
                provider => provider.GetService(typeof(KontecgRoleStore<TRole, TUser>)));

            //KontecgFeatureValueStore
            services.TryAddScoped<KontecgFeatureValueStore<TCompany, TUser>>();
            services.TryAddScoped(typeof(IFeatureValueStore),
                provider => provider.GetService(typeof(KontecgFeatureValueStore<TCompany, TUser>)));

            IdentityBuilder builder = services.AddIdentityCore<TUser>(setupAction);

            return new KontecgIdentityBuilder(builder, typeof(TRole), typeof(TCompany));
        }
    }
}
