using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddKontecgDbContextOptionsExtension(this DbContextOptionsBuilder optionsBuilder)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new KontecgDbContextOptionsExtension());
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> AddKontecgDbContextOptionsExtension<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder)
            where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new KontecgDbContextOptionsExtension());
            return optionsBuilder;
        }
    }
}
