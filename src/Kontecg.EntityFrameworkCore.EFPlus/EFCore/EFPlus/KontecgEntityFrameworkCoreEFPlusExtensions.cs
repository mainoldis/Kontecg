using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Linq.Expressions;
using Kontecg.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Extensions;
using Z.EntityFramework.Plus;

namespace Kontecg.EFCore.EFPlus
{
    /// <summary>
    ///     Defines batch delete and update extension methods for IRepository
    /// </summary>
    public static class KontecgEntityFrameworkCoreEfPlusExtensions
    {
        /// <summary>
        ///     Deletes all matching entities permanently for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <param name="batchDeleteBuilder">The batch delete builder to change default configuration.</param>
        /// <returns></returns>
        public static async Task<int> BatchDeleteAsync<TEntity, TPrimaryKey>(
            [NotNull] this IRepository<TEntity, TPrimaryKey> repository,
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            Action<BatchDelete> batchDeleteBuilder = null)
            where TEntity : Entity<TPrimaryKey>
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(predicate, nameof(predicate));

            IQueryable<TEntity> query = repository.GetAll().IgnoreQueryFilters();

            Expression<Func<TEntity, bool>> kontecgFilterExpression =
                GetFilterExpressionOrNull<TEntity, TPrimaryKey>(repository.GetIocResolver());
            Expression<Func<TEntity, bool>> filterExpression =
                ExpressionCombiner.Combine(predicate, kontecgFilterExpression);

            query = query.Where(filterExpression);

            return await query.DeleteAsync(batchDeleteBuilder);
        }

        /// <summary>
        ///     Deletes all matching entities permanently for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <param name="batchDeleteBuilder">The batch delete builder to change default configuration.</param>
        /// <returns></returns>
        public static async Task<int> BatchDeleteAsync<TEntity>(
            [NotNull] this IRepository<TEntity> repository,
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            Action<BatchDelete> batchDeleteBuilder = null)
            where TEntity : Entity<int>
        {
            return await repository.BatchDeleteAsync<TEntity, int>(predicate, batchDeleteBuilder);
        }

        /// <summary>
        ///     Updates all matching entities using given updateExpression for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type</typeparam>
        /// <param name="repository">Repository</param>
        /// ///
        /// <param name="updateExpression">Update expression</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <param name="batchUpdateBuilder">The batch delete builder to change default configuration.</param>
        /// <returns></returns>
        public static async Task<int> BatchUpdateAsync<TEntity, TPrimaryKey>(
            [NotNull] this IRepository<TEntity, TPrimaryKey> repository,
            [NotNull] Expression<Func<TEntity, TEntity>> updateExpression,
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            Action<BatchUpdate> batchUpdateBuilder = null)
            where TEntity : Entity<TPrimaryKey>
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(updateExpression, nameof(updateExpression));
            Check.NotNull(predicate, nameof(predicate));

            IQueryable<TEntity> query = repository.GetAll().IgnoreQueryFilters();

            Expression<Func<TEntity, bool>> kontecgFilterExpression =
                GetFilterExpressionOrNull<TEntity, TPrimaryKey>(repository.GetIocResolver());
            Expression<Func<TEntity, bool>> filterExpression =
                ExpressionCombiner.Combine(predicate, kontecgFilterExpression);

            query = query.Where(filterExpression);

            return await query.UpdateAsync(updateExpression, batchUpdateBuilder);
        }

        /// <summary>
        ///     Updates all matching entities using given updateExpression for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="repository">Repository</param>
        /// ///
        /// <param name="updateExpression">Update expression</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <param name="batchUpdateBuilder">The batch delete builder to change default configuration.</param>
        /// <returns></returns>
        public static async Task<int> BatchUpdateAsync<TEntity>(
            [NotNull] this IRepository<TEntity> repository,
            [NotNull] Expression<Func<TEntity, TEntity>> updateExpression,
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            Action<BatchUpdate> batchUpdateBuilder = null)
            where TEntity : Entity<int>
        {
            return await repository.BatchUpdateAsync<TEntity, int>(updateExpression, predicate, batchUpdateBuilder);
        }

        private static Expression<Func<TEntity, bool>> GetFilterExpressionOrNull<TEntity, TPrimaryKey>(
            IIocResolver iocResolver)
            where TEntity : Entity<TPrimaryKey>
        {
            Expression<Func<TEntity, bool>> expression = null;

            using IScopedIocResolver scope = iocResolver.CreateScope();
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider = scope.Resolve<ICurrentUnitOfWorkProvider>();

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                bool isSoftDeleteFilterEnabled =
                    currentUnitOfWorkProvider.Current?.IsFilterEnabled(KontecgDataFilters.SoftDelete) == true;
                if (isSoftDeleteFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete) e).IsDeleted;
                    expression = softDeleteFilter;
                }
            }

            if (typeof(IMayHaveCompany).IsAssignableFrom(typeof(TEntity)))
            {
                bool isMayHaveCompanyFilterEnabled =
                    currentUnitOfWorkProvider.Current?.IsFilterEnabled(KontecgDataFilters.MayHaveCompany) == true;
                int? currentCompanyId = GetCurrentCompanyIdOrNull(iocResolver);

                if (isMayHaveCompanyFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> mayHaveCompanyFilter =
                        e => ((IMayHaveCompany) e).CompanyId == currentCompanyId;
                    expression = expression == null
                        ? mayHaveCompanyFilter
                        : ExpressionCombiner.Combine(expression, mayHaveCompanyFilter);
                }
            }

            if (typeof(IMustHaveCompany).IsAssignableFrom(typeof(TEntity)))
            {
                bool isMustHaveCompanyFilterEnabled =
                    currentUnitOfWorkProvider.Current?.IsFilterEnabled(KontecgDataFilters.MustHaveCompany) == true;
                int? currentCompanyId = GetCurrentCompanyIdOrNull(iocResolver);

                if (isMustHaveCompanyFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> mustHaveCompanyFilter =
                        e => ((IMustHaveCompany) e).CompanyId == currentCompanyId;
                    expression = expression == null
                        ? mustHaveCompanyFilter
                        : ExpressionCombiner.Combine(expression, mustHaveCompanyFilter);
                }
            }

            return expression;
        }

        private static int? GetCurrentCompanyIdOrNull(IIocResolver iocResolver)
        {
            using IScopedIocResolver scope = iocResolver.CreateScope();
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider = scope.Resolve<ICurrentUnitOfWorkProvider>();

            if (currentUnitOfWorkProvider?.Current != null)
            {
                return currentUnitOfWorkProvider.Current.GetCompanyId();
            }

            return iocResolver.Resolve<IKontecgSession>().CompanyId;
        }
    }
}
