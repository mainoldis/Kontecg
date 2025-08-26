using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
namespace Kontecg.EFCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ConfigureSoftDeleteDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, KontecgEfCoreCurrentDbContext kontecgEfCoreCurrentDbContext)
        {
            modelBuilder.HasDbFunction(methodInfo)
                .HasTranslation(args =>
                {
                    // (bool isDeleted, bool boolParam)
                    var isDeleted = args[0];
                    var boolParam = args[1];

                    if (kontecgEfCoreCurrentDbContext.Context?.IsSoftDeleteFilterEnabled == true)
                    {
                        // IsDeleted == false
                        return new SqlBinaryExpression(
                            ExpressionType.Equal,
                            isDeleted,
                            new SqlConstantExpression(Expression.Constant(false), boolParam.TypeMapping),
                            boolParam.Type,
                            boolParam.TypeMapping);
                    }

                    // empty where sql
                    return new SqlConstantExpression(Expression.Constant(true), boolParam.TypeMapping);
                });

            return modelBuilder;
        }

        public static ModelBuilder ConfigureMayHaveCompanyDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, KontecgEfCoreCurrentDbContext kontecgEfCoreCurrentDbContext)
        {
            modelBuilder.HasDbFunction(methodInfo)
                .HasTranslation(args =>
                {
                    // (int? companyId, int? currentCompanyId, bool boolParam)
                    var companyId = args[0];
                    var currentCompanyId = args[1];
                    var boolParam = args[2];

                    if (kontecgEfCoreCurrentDbContext.Context?.IsMayHaveCompanyFilterEnabled == true)
                    {
                        // CompanyId == CurrentCompanyId
                        return new SqlBinaryExpression(
                            ExpressionType.Equal,
                            companyId,
                            currentCompanyId,
                            boolParam.Type,
                            boolParam.TypeMapping);
                    }

                    // empty where sql
                    return new SqlConstantExpression(Expression.Constant(true), boolParam.TypeMapping);
                });

            return modelBuilder;
        }

        public static ModelBuilder ConfigureMustHaveCompanyDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, KontecgEfCoreCurrentDbContext kontecgEfCoreCurrentDbContext)
        {
            modelBuilder.HasDbFunction(methodInfo)
                .HasTranslation(args =>
                {
                    // (int? companyId, int? currentCompanyId, bool boolParam)
                    var companyId = args[0];
                    var currentCompanyId = args[1];
                    var boolParam = args[2];

                    if (kontecgEfCoreCurrentDbContext.Context?.IsMustHaveCompanyFilterEnabled == true)
                    {
                        // CompanyId == CurrentCompanyId
                        return new SqlBinaryExpression(
                            ExpressionType.Equal,
                            companyId,
                            currentCompanyId,
                            boolParam.Type,
                            boolParam.TypeMapping);
                    }

                    // empty where sql
                    return new SqlConstantExpression(Expression.Constant(true), boolParam.TypeMapping);
                });

            return modelBuilder;
        }
    }
}
