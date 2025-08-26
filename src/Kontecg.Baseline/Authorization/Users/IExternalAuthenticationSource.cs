using System.Threading.Tasks;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization.Users
{
    /// <summary>
    ///     Defines an external authorization source.
    /// </summary>
    /// <typeparam name="TCompany">Company type</typeparam>
    /// <typeparam name="TUser">User type</typeparam>
    public interface IExternalAuthenticationSource<TCompany, TUser>
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase
    {
        /// <summary>
        ///     Unique name of the authentication source.
        ///     This source name is set to <see cref="KontecgUserBase.AuthenticationSource" />
        ///     if the user authenticated by this authentication source
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Used to try authenticate a user by this source.
        /// </summary>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <param name="plainPassword">Plain password of the user</param>
        /// <param name="company">Company of the user or null (if user is a host user)</param>
        /// <returns>True, indicates that this used has authenticated by this source</returns>
        Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, TCompany company);

        /// <summary>
        ///     This method is a user authenticated by this source which does not exists yet.
        ///     So, source should create the User and fill properties.
        /// </summary>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <param name="company">Company of the user or null (if user is a host user)</param>
        /// <returns>Newly created user</returns>
        Task<TUser> CreateUserAsync(string userNameOrEmailAddress, TCompany company);

        /// <summary>
        ///     This method is called after an existing user is authenticated by this source.
        ///     It can be used to update some properties of the user by the source.
        /// </summary>
        /// <param name="user">The user that can be updated</param>
        /// <param name="company">Company of the user or null (if user is a host user)</param>
        Task UpdateUserAsync(TUser user, TCompany company);

        /// <summary>
        ///     Used to try authenticate a user by this source.
        /// </summary>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <param name="plainPassword">Plain password of the user</param>
        /// <param name="company">Company of the user or null (if user is a host user)</param>
        /// <returns>True, indicates that this used has authenticated by this source</returns>
        bool TryAuthenticate(string userNameOrEmailAddress, string plainPassword, TCompany company);

        /// <summary>
        ///     This method is a user authenticated by this source which does not exists yet.
        ///     So, source should create the User and fill properties.
        /// </summary>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <param name="company">Company of the user or null (if user is a host user)</param>
        /// <returns>Newly created user</returns>
        TUser CreateUser(string userNameOrEmailAddress, TCompany company);

        /// <summary>
        ///     This method is called after an existing user is authenticated by this source.
        ///     It can be used to update some properties of the user by the source.
        /// </summary>
        /// <param name="user">The user that can be updated</param>
        /// <param name="company">Company of the user or null (if user is a host user)</param>
        void UpdateUser(TUser user, TCompany company);
    }
}
