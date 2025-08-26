using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq.Expressions;
using System.Threading;

namespace Kontecg.EFCore
{
#pragma warning disable EF1001
    public class KontecgEntityQueryProvider : EntityQueryProvider
    {
        protected KontecgEfCoreCurrentDbContext KontecgEfCoreCurrentDbContext { get; }
        protected ICurrentDbContext CurrentDbContext { get; }

        public KontecgEntityQueryProvider(
            [NotNull] IQueryCompiler queryCompiler,
            KontecgEfCoreCurrentDbContext kontecgEfCoreCurrentDbContext,
            ICurrentDbContext currentDbContext)
            : base(queryCompiler)
        {
            KontecgEfCoreCurrentDbContext = kontecgEfCoreCurrentDbContext;
            CurrentDbContext = currentDbContext;
        }

        public override object Execute(Expression expression)
        {
            using (KontecgEfCoreCurrentDbContext.Use(CurrentDbContext.Context as KontecgDbContext))
            {
                return base.Execute(expression);
            }
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            using (KontecgEfCoreCurrentDbContext.Use(CurrentDbContext.Context as KontecgDbContext))
            {
                return base.Execute<TResult>(expression);
            }
        }

        public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
        {
            using (KontecgEfCoreCurrentDbContext.Use(CurrentDbContext.Context as KontecgDbContext))
            {
                return base.ExecuteAsync<TResult>(expression, cancellationToken);
            }
        }
    }
#pragma warning restore EF1001
}
