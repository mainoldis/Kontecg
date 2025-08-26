using System;
using Kontecg.Application.Clients;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.IdentityFramework;
using Kontecg.MultiCompany;
using Microsoft.AspNetCore.Identity;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class KontecgBaselineIdentityBuilderExtensions
    {
        public static KontecgIdentityBuilder AddKontecgCompanyManager<TCompanyManager>(
            this KontecgIdentityBuilder builder)
            where TCompanyManager : class
        {
            Type type = typeof(TCompanyManager);
            Type kontecgManagerType =
                typeof(KontecgCompanyManager<,>).MakeGenericType(builder.CompanyType, builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetRequiredService(kontecgManagerType));
            builder.Services.AddScoped(kontecgManagerType, type);
            return builder;
        }

        public static KontecgIdentityBuilder AddKontecgClientManager<TClientManager>(
            this KontecgIdentityBuilder builder)
            where TClientManager : class
        {
            Type type = typeof(TClientManager);
            Type kontecgManagerType =
                typeof(KontecgClientManager<,>).MakeGenericType(builder.CompanyType, builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetRequiredService(kontecgManagerType));
            builder.Services.AddScoped(kontecgManagerType, type);
            return builder;
        }

        public static KontecgIdentityBuilder AddKontecgRoleManager<TRoleManager>(this KontecgIdentityBuilder builder)
            where TRoleManager : class
        {
            Type kontecgManagerType = typeof(KontecgRoleManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            Type managerType = typeof(RoleManager<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(kontecgManagerType, services => services.GetRequiredService(managerType));
            builder.AddRoleManager<TRoleManager>();
            return builder;
        }

        public static KontecgIdentityBuilder AddKontecgUserManager<TUserManager>(this KontecgIdentityBuilder builder)
            where TUserManager : class
        {
            Type kontecgManagerType = typeof(KontecgUserManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            Type managerType = typeof(UserManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(kontecgManagerType, services => services.GetRequiredService(managerType));
            builder.AddUserManager<TUserManager>();
            return builder;
        }

        public static KontecgIdentityBuilder AddKontecgLogInManager<TLogInManager>(this KontecgIdentityBuilder builder)
            where TLogInManager : class
        {
            Type type = typeof(TLogInManager);
            Type kontecgManagerType =
                typeof(KontecgLogInManager<,,>).MakeGenericType(builder.CompanyType, builder.RoleType,
                    builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetService(kontecgManagerType));
            builder.Services.AddScoped(kontecgManagerType, type);
            return builder;
        }

        public static KontecgIdentityBuilder AddKontecgUserClaimsPrincipalFactory<TUserClaimsPrincipalFactory>(
            this KontecgIdentityBuilder builder)
            where TUserClaimsPrincipalFactory : class
        {
            Type type = typeof(TUserClaimsPrincipalFactory);
            builder.Services.AddScoped(
                typeof(UserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType),
                services => services.GetRequiredService(type));
            builder.Services.AddScoped(
                typeof(KontecgUserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType),
                services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType),
                services => services.GetRequiredService(type));
            builder.Services.AddScoped(type);
            return builder;
        }

        public static KontecgIdentityBuilder AddPermissionChecker<TPermissionChecker>(
            this KontecgIdentityBuilder builder)
            where TPermissionChecker : class
        {
            Type type = typeof(TPermissionChecker);
            Type checkerType = typeof(PermissionChecker<,>).MakeGenericType(builder.RoleType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(checkerType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IPermissionChecker), provider => provider.GetService(type));
            return builder;
        }

        public static KontecgIdentityBuilder AddKontecgUserStore<TUserStore>(this KontecgIdentityBuilder builder)
            where TUserStore : class
        {
            Type type = typeof(TUserStore);
            Type kontecgStoreType = typeof(KontecgUserStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            Type storeType = typeof(IUserStore<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(kontecgStoreType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static KontecgIdentityBuilder AddKontecgRoleStore<TRoleStore>(this KontecgIdentityBuilder builder)
            where TRoleStore : class
        {
            Type type = typeof(TRoleStore);
            Type kontecgStoreType = typeof(KontecgRoleStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            Type storeType = typeof(IRoleStore<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(kontecgStoreType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static KontecgIdentityBuilder AddFeatureValueStore<TFeatureValueStore>(
            this KontecgIdentityBuilder builder)
            where TFeatureValueStore : class
        {
            Type type = typeof(TFeatureValueStore);
            Type storeType = typeof(KontecgFeatureValueStore<,>).MakeGenericType(builder.CompanyType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(storeType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IFeatureValueStore), provider => provider.GetService(type));
            return builder;
        }
    }
}
