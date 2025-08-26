using System;
using Castle.MicroKernel.Registration;
using Kontecg.Dependency;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Configuration
{
    public class KontecgEfCoreConfiguration : IKontecgEfCoreConfiguration
    {
        private readonly IIocManager _iocManager;

        public KontecgEfCoreConfiguration(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        public bool UseKontecgQueryCompiler { get; set; } = false;

        public void AddDbContext<TDbContext>(Action<KontecgDbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext
        {
            _iocManager.IocContainer.Register(
                Component.For<IKontecgDbContextConfigurer<TDbContext>>().Instance(
                    new KontecgDbContextConfigurerAction<TDbContext>(action)
                ).IsDefault()
            );
        }
    }
}
