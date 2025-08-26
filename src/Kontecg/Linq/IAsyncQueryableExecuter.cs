using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kontecg.Linq
{
    /// <summary>
    ///     This interface is intended to be used by Kontecg.
    /// </summary>
    public interface IAsyncQueryableExecuter
    {
        Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

        Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);
    }
}
