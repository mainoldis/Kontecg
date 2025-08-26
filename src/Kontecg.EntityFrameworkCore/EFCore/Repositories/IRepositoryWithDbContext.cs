using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();

        Task<DbContext> GetDbContextAsync();
    }
}
