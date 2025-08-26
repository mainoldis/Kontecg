using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Configuration
{
    public interface IKontecgDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        void Configure(KontecgDbContextConfiguration<TDbContext> configuration);
    }
}
