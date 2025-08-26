using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     Permission manager.
    /// </summary>
    public interface IPermissionManager
    {
        /// <summary>
        /// Gets <see cref="Permission"/> object with given <paramref name="name"/> or throws exception
        /// if there is no permission with given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        Permission GetPermission(string name);

        /// <summary>
        /// Gets <see cref="Permission"/> object with given <paramref name="name"/> or returns null
        /// if there is no permission with given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        Permission GetPermissionOrNull(string name);

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        /// <param name="companyFilter">Can be passed false to disable company filter.</param>
        IReadOnlyList<Permission> GetAllPermissions(bool companyFilter = true);

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        /// <param name="companyFilter">Can be passed false to disable company filter.</param>
        Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(bool companyFilter = true);

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        /// <param name="multiCompanySides">Multi-company side to filter</param>
        IReadOnlyList<Permission> GetAllPermissions(MultiCompanySides multiCompanySides);

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        /// <param name="multiCompanySides">Multi-company side to filter</param>
        Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(MultiCompanySides multiCompanySides);
    }
}
