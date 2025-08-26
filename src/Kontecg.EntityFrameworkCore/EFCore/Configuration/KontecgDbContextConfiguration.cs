using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Configuration
{
    public class KontecgDbContextConfiguration<TDbContext>
        where TDbContext : DbContext
    {
        public KontecgDbContextConfiguration(string connectionString, DbConnection existingConnection)
        {
            ConnectionString = connectionString;
            ExistingConnection = existingConnection;

            DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
        }

        public string ConnectionString { get; internal set; }

        public DbConnection ExistingConnection { get; internal set; }

        public DbContextOptionsBuilder<TDbContext> DbContextOptions { get; }
    }
}
