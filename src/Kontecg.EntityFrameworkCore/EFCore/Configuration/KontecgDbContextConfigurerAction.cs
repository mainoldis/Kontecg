using System;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Configuration
{
    public class KontecgDbContextConfigurerAction<TDbContext> : IKontecgDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        public KontecgDbContextConfigurerAction(Action<KontecgDbContextConfiguration<TDbContext>> action)
        {
            Action = action;
        }

        public Action<KontecgDbContextConfiguration<TDbContext>> Action { get; set; }

        public void Configure(KontecgDbContextConfiguration<TDbContext> configuration)
        {
            Action(configuration);
        }
    }
}
