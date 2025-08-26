using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kontecg.EFCore
{
    public class KontecgDbContextOptionsExtension : IDbContextOptionsExtension
    {
        public void ApplyServices(IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ICompiledQueryCacheKeyGenerator));
            if (serviceDescriptor != null && serviceDescriptor.ImplementationType != null)
            {
                services.Remove(serviceDescriptor);
                services.AddScoped(serviceDescriptor.ImplementationType);
                services.Add(ServiceDescriptor.Scoped<ICompiledQueryCacheKeyGenerator>(provider =>
                    ActivatorUtilities.CreateInstance<KontecgCompiledQueryCacheKeyGenerator>(provider,
                        provider.GetRequiredService(serviceDescriptor.ImplementationType)
                            .As<ICompiledQueryCacheKeyGenerator>())));
            }

            services.Replace(ServiceDescriptor.Scoped<IAsyncQueryProvider, KontecgEntityQueryProvider>());
            services.AddSingleton<KontecgEfCoreCurrentDbContext>();
        }

        public void Validate(IDbContextOptions options)
        {
        }

        public DbContextOptionsExtensionInfo Info => new KontecgOptionsExtensionInfo(this);

        private class KontecgOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public KontecgOptionsExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override bool IsDatabaseProvider => false;

            public override int GetServiceProviderHashCode() => 0;

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is KontecgOptionsExtensionInfo;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
            }

            public override string LogFragment => "KontecgOptionsExtension";
        }
    }
}
