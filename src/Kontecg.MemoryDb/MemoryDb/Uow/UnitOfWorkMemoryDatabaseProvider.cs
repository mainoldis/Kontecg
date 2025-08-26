using Kontecg.Dependency;
using Kontecg.Domain.Uow;

namespace Kontecg.MemoryDb.Uow
{
    /// <summary>
    ///     Implements <see cref="IMemoryDatabaseProvider" /> that gets database from active unit of work.
    /// </summary>
    public class UnitOfWorkMemoryDatabaseProvider : IMemoryDatabaseProvider, ITransientDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWork;

        public UnitOfWorkMemoryDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            _currentUnitOfWork = currentUnitOfWork;
        }

        public MemoryDatabase Database => ((MemoryDbUnitOfWork) _currentUnitOfWork.Current).Database;
    }
}
