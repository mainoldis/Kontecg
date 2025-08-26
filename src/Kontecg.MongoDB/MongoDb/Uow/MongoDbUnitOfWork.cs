using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.MongoDb.Configuration;
using MongoDB.Driver;

namespace Kontecg.MongoDb.Uow
{
    /// <summary>
    ///     Implements Unit of work for MongoDB.
    /// </summary>
    public class MongoDbUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly IKontecgMongoDbModuleConfiguration _configuration;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public MongoDbUnitOfWork(
            IKontecgMongoDbModuleConfiguration configuration,
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkFilterExecuter filterExecuter,
            IUnitOfWorkDefaultOptions defaultOptions)
            : base(
                connectionStringResolver,
                defaultOptions,
                filterExecuter)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///     Gets a reference to MongoDB Database.
        /// </summary>
        public IMongoDatabase Database { get; private set; }

#pragma warning disable
        protected override void BeginUow()
        {
            Database = new MongoClient(_configuration.ConnectionString)
                .GetDatabase(_configuration.DatabaseName);
        }
#pragma warning restore

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
