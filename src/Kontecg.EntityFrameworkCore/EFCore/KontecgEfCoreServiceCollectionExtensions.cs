using System;
using Kontecg.EFCore.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.EFCore
{
    public static class KontecgEfCoreServiceCollectionExtensions
    {
        public static void AddKontecgDbContext<TDbContext>(
            this IServiceCollection services,
            Action<KontecgDbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext
        {
            services.AddSingleton(
                typeof(IKontecgDbContextConfigurer<TDbContext>),
                new KontecgDbContextConfigurerAction<TDbContext>(action)
            );
        }
    }
}
