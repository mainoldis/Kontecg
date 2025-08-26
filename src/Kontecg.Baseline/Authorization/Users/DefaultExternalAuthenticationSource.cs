using System.Threading.Tasks;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     This is a helper base class to easily update <see cref="IExternalAuthenticationSource{TCompany,TUser}" />.
    ///     Implements some methods as default, but you can override all methods.
    /// </summary>
    /// <typeparam name="TCompany">Company type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    public abstract class
        DefaultExternalAuthenticationSource<TCompany, TUser> : IExternalAuthenticationSource<TCompany, TUser>
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase, new()
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword,
            TCompany company);

        /// <inheritdoc />
        public virtual Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TCompany company)
        {
            return Task.FromResult(
                new TUser
                {
                    UserName = userNameOrEmailAddress,
                    Name = userNameOrEmailAddress,
                    Surname = userNameOrEmailAddress,
                    EmailAddress = userNameOrEmailAddress,
                    IsEmailConfirmed = true,
                    IsActive = true
                });
        }

        /// <inheritdoc />
        public virtual Task UpdateUserAsync(TUser user, TCompany company)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public abstract bool TryAuthenticate(string userNameOrEmailAddress, string plainPassword, TCompany company);

        /// <inheritdoc />
        public virtual TUser CreateUser(string userNameOrEmailAddress, TCompany company)
        {
            return new TUser
            {
                UserName = userNameOrEmailAddress,
                Name = userNameOrEmailAddress,
                Surname = userNameOrEmailAddress,
                EmailAddress = userNameOrEmailAddress,
                IsEmailConfirmed = true,
                IsActive = true
            };
        }

        /// <inheritdoc />
        public virtual void UpdateUser(TUser user, TCompany company)
        {
        }
    }
}
