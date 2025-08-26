using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Kontecg.Data;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;

namespace Kontecg.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : DapperRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>

    {
        private readonly IActiveTransactionProvider _activeTransactionProvider;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(activeTransactionProvider)
        {
            _activeTransactionProvider = activeTransactionProvider;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public ActiveTransactionProviderArgs ActiveTransactionProviderArgs =>
            new ActiveTransactionProviderArgs
            {
                ["ContextType"] = typeof(TDbContext),
                ["MultiCompanySide"] = MultiCompanySide
            };

        public override int? Timeout => _currentUnitOfWorkProvider.Current?.Options.Timeout?.TotalSeconds.To<int>();

        public override DbConnection GetConnection()
        {
            return (DbConnection) _activeTransactionProvider.GetActiveConnection(ActiveTransactionProviderArgs);
        }

        public override async Task<DbConnection> GetConnectionAsync()
        {
            IDbConnection connection = await _activeTransactionProvider
                .GetActiveConnectionAsync(ActiveTransactionProviderArgs);

            return (DbConnection) connection;
        }

        public override DbTransaction GetActiveTransaction()
        {
            return (DbTransaction) _activeTransactionProvider.GetActiveTransaction(ActiveTransactionProviderArgs);
        }

        public override async Task<DbTransaction> GetActiveTransactionAsync()
        {
            IDbTransaction transaction = await _activeTransactionProvider
                .GetActiveTransactionAsync(ActiveTransactionProviderArgs);

            return (DbTransaction) transaction;
        }
    }
}
