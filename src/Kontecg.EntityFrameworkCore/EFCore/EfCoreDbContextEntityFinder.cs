using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    internal class EfCoreDbContextEntityFinder : IDbContextEntityFinder, ITransientDependency
    {
        public IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType)
        {
            return
                from property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType.GenericTypeArguments[0],
                        typeof(IEntity<>))
                select new EntityTypeInfo(property.PropertyType.GenericTypeArguments[0], property.DeclaringType);
        }
    }
}
