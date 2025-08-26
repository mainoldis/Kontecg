using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Baseline.ServicioAdmin.Authentication.Native;
using Kontecg.Baseline.ServicioAdmin.Configuration;
using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.MultiCompany;

namespace Kontecg.Baseline.ServicioAdmin.Authentication
{
    /// <summary>
    ///     Implements <see cref="IExternalAuthenticationSource{TCompany,TUser}" /> to authenticate users from ServicioAdmin.
    ///     Extend this class using application's User and Company classes as type parameters.
    ///     Also, all needed methods can be overridden and changed upon your needs.
    /// </summary>
    /// <typeparam name="TCompany">Company type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    public abstract class ServicioAdminAuthenticationSource<TCompany, TUser> :
        DefaultExternalAuthenticationSource<TCompany, TUser>,
        ITransientDependency
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase, new()
    {
        /// <summary>
        ///     CHENET
        /// </summary>
        public const string SourceName = "CHENET";

        private readonly IKontecgServicioAdminModuleConfig _servicioAdminModuleConfig;

        private readonly IServicioAdminSettings _settings;

        protected ServicioAdminAuthenticationSource(
            IServicioAdminSettings settings,
            IKontecgServicioAdminModuleConfig servicioAdminModuleConfig)
        {
            _settings = settings;
            _servicioAdminModuleConfig = servicioAdminModuleConfig;
        }

        public override string Name => SourceName;

        /// <inheritdoc />
        public override async Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword,
            TCompany Company)
        {
            if (!_servicioAdminModuleConfig.IsEnabled ||
                !await _settings.GetIsEnabledAsync(Company?.Id) ||
                (_servicioAdminModuleConfig.IsEnabled &&
                 _servicioAdminModuleConfig.CompanyName != Company?.CompanyName))
            {
                return false;
            }

            try
            {
                using Db db = new Db("");
                IEnumerable<dynamic> enumerable = await db.Sql("")
                    .WithParameters(new {userNameOrEmailAddress, plainPassword})
                    .AsEnumerableAsync();
                return enumerable.Any();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override async Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TCompany company)
        {
            await CheckIsEnabledAsync(company);

            TUser user = await base.CreateUserAsync(userNameOrEmailAddress, company);

            return user;
        }

        public override async Task UpdateUserAsync(TUser user, TCompany Company)
        {
            await CheckIsEnabledAsync(Company);
            await base.UpdateUserAsync(user, Company);
        }

        protected virtual async Task CheckIsEnabledAsync(TCompany Company)
        {
            if (!_servicioAdminModuleConfig.IsEnabled)
            {
                throw new KontecgException("ServicioAdmin Authentication module is disabled globally!");
            }

            if (_servicioAdminModuleConfig.CompanyName != Company?.CompanyName)
            {
                throw new KontecgException("ServicioAdmin Authentication is disabled for given Company (Name:" +
                                           Company?.CompanyName +
                                           ")! It's only enabled for '" + _servicioAdminModuleConfig.CompanyName + "'");
            }

            int? companyId = Company?.Id;
            if (!await _settings.GetIsEnabledAsync(companyId))
            {
                throw new KontecgException("ServicioAdmin Authentication is disabled for given Company (id:" +
                                           companyId +
                                           ")! You can enable it by setting '" + ServicioAdminSettingNames.IsEnabled +
                                           "' to true");
            }
        }

        protected static string ConvertToNullIfEmpty(string str)
        {
            return str.IsNullOrWhiteSpace()
                ? null
                : str;
        }
    }
}
