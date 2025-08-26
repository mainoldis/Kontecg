using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.MemoryDb.Configuration;

namespace Kontecg.MemoryDb.Uow
{
    /// <summary>
    ///     Implements Unit of work for MemoryDb.
    /// </summary>
    public class MemoryDbUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly IKontecgMemoryDbModuleConfiguration _configuration;
        private readonly MemoryDatabase _memoryDatabase;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public MemoryDbUnitOfWork(
            IKontecgMemoryDbModuleConfiguration configuration,
            MemoryDatabase memoryDatabase,
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkFilterExecuter filterExecuter,
            IUnitOfWorkDefaultOptions defaultOptions)
            : base(
                connectionStringResolver,
                defaultOptions,
                filterExecuter)
        {
            _configuration = configuration;
            _memoryDatabase = memoryDatabase;
        }

        /// <summary>
        ///     Gets a reference to Memory Database.
        /// </summary>
        public MemoryDatabase Database { get; private set; }

        protected override void BeginUow()
        {
            Database = _memoryDatabase;
        }

        public override void SaveChanges()
        {
        }

#pragma warning disable 1998
        public override async Task SaveChangesAsync()
        {
        }
#pragma warning restore 1998

        protected override void CompleteUow()
        {
        }

#pragma warning disable 1998
        protected override async Task CompleteUowAsync()
        {
        }
#pragma warning restore 1998

        protected override void DisposeUow()
        {
        }
    }
}
