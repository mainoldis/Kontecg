using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using MongoDB.Driver;

namespace Kontecg.MongoDb.Uow
{
    /// <summary>
    ///     Implements <see cref="IMongoDatabaseProvider" /> that gets database from active unit of work.
    /// </summary>
    public class UnitOfWorkMongoDatabaseProvider : IMongoDatabaseProvider, ITransientDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWork;

        public UnitOfWorkMongoDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            _currentUnitOfWork = currentUnitOfWork;
        }

        public IMongoDatabase Database => ((MongoDbUnitOfWork) _currentUnitOfWork.Current).Database;
    }
}
