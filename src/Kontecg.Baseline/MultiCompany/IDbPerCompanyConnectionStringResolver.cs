using System.Threading.Tasks;
using Kontecg.Domain.Uow;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Extends <see cref="IConnectionStringResolver" /> to
    ///     get connection string for given company.
    /// </summary>
    public interface IDbPerCompanyConnectionStringResolver : IConnectionStringResolver
    {
        /// <summary>
        ///     Gets the connection string for given args.
        /// </summary>
        string GetNameOrConnectionString(DbPerCompanyConnectionStringResolveArgs args);

        /// <summary>
        ///     Gets the connection string for given args.
        /// </summary>
        Task<string> GetNameOrConnectionStringAsync(DbPerCompanyConnectionStringResolveArgs args);
    }
}
