using System;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Configuration
{
    public interface IKontecgEfCoreConfiguration
    {
        public bool UseKontecgQueryCompiler { get; set; }

        void AddDbContext<TDbContext>(Action<KontecgDbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext;
    }

    public class NullKontecgEfCoreConfiguration : IKontecgEfCoreConfiguration
    {
        /// <summary>
        /// Gets single instance of <see cref="NullKontecgEfCoreConfiguration"/> class.
        /// </summary>
        public static NullKontecgEfCoreConfiguration Instance { get; } = new NullKontecgEfCoreConfiguration();

        public bool UseKontecgQueryCompiler { get; set; }

        public void AddDbContext<TDbContext>(Action<KontecgDbContextConfiguration<TDbContext>> action) where TDbContext : DbContext
        {
        }
    }
}
