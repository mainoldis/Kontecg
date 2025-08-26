using System.Threading.Tasks;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    public sealed class SimpleDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public SimpleDbContextProvider(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public TDbContext DbContext { get; }

        public Task<TDbContext> GetDbContextAsync()
        {
            return Task.FromResult(DbContext);
        }

        public Task<TDbContext> GetDbContextAsync(MultiCompanySides? multiCompanySide)
        {
            return Task.FromResult(DbContext);
        }

        public TDbContext GetDbContext()
        {
            return DbContext;
        }

        public TDbContext GetDbContext(MultiCompanySides? multiCompanySide)
        {
            return DbContext;
        }
    }
}
