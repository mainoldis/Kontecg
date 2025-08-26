using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DapperExtensions;
using Kontecg.Dapper.Utils;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;

namespace Kontecg.Dapper.Filters.Query
{
    public class MustHaveCompanyDapperQueryFilter : IDapperQueryFilter
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public MustHaveCompanyDapperQueryFilter(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        private int CompanyId
        {
            get
            {
                DataFilterConfiguration filter =
                    _currentUnitOfWorkProvider.Current.Filters.FirstOrDefault(x => x.FilterName == FilterName);
                if (filter.FilterParameters.ContainsKey(KontecgDataFilters.Parameters.CompanyId))
                {
                    return (int) filter.FilterParameters[KontecgDataFilters.Parameters.CompanyId];
                }

                return MultiCompanyConsts.DefaultCompanyId;
            }
        }

        public string FilterName { get; } = KontecgDataFilters.MustHaveCompany;

        public bool IsEnabled => _currentUnitOfWorkProvider.Current.IsFilterEnabled(FilterName);

        public IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            IFieldPredicate predicate = null;
            if (typeof(IMustHaveCompany).IsAssignableFrom(typeof(TEntity)) && IsEnabled)
            {
                predicate = Predicates.Field<TEntity>(f => (f as IMustHaveCompany).CompanyId, Operator.Eq, CompanyId);
            }

            return predicate;
        }

        public Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (typeof(IMustHaveCompany).IsAssignableFrom(typeof(TEntity)) && IsEnabled)
            {
                PropertyInfo propType = typeof(TEntity).GetProperty(nameof(IMustHaveCompany.CompanyId));
                if (predicate == null)
                {
                    predicate = ExpressionUtils.MakePredicate<TEntity>(nameof(IMustHaveCompany.CompanyId), CompanyId,
                        propType.PropertyType);
                }
                else
                {
                    ParameterExpression paramExpr = predicate.Parameters[0];
                    MemberExpression memberExpr = Expression.Property(paramExpr, nameof(IMustHaveCompany.CompanyId));
                    BinaryExpression body = Expression.AndAlso(
                        predicate.Body,
                        Expression.Equal(memberExpr, Expression.Constant(CompanyId, propType.PropertyType)));
                    predicate = Expression.Lambda<Func<TEntity, bool>>(body, paramExpr);
                }
            }

            return predicate;
        }
    }
}
