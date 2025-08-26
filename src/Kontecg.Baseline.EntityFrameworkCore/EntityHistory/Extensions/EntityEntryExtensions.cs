using Kontecg.Domain.Entities;
using Kontecg.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Kontecg.EntityHistory.Extensions
{
    internal static class EntityEntryExtensions
    {
        internal static bool IsCreated(this EntityEntry entityEntry)
        {
            return entityEntry.State == EntityState.Added;
        }

        internal static bool IsDeleted(this EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Deleted)
            {
                return true;
            }

            object entity = entityEntry.Entity;
            return entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted;
        }
    }
}
