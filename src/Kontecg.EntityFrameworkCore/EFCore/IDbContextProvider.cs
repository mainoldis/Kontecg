using System.Threading.Tasks;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    public interface IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        Task<TDbContext> GetDbContextAsync();

        Task<TDbContext> GetDbContextAsync(MultiCompanySides? multiCompanySide);

        TDbContext GetDbContext();

        TDbContext GetDbContext(MultiCompanySides? multiCompanySide);
    }
}
