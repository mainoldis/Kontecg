using System;
using System.Threading.Tasks;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Uow
{
    /// <summary>
    ///     Extension methods for UnitOfWork.
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        ///     Gets a DbContext as a part of active unit of work.
        ///     This method can be called when current unit of work is an <see cref="EfCoreUnitOfWork" />.
        /// </summary>
        /// <typeparam name="TDbContext">Type of the DbContext</typeparam>
        /// <param name="unitOfWork">Current (active) unit of work</param>
        /// <param name="multiCompanySide">Multicompany side</param>
        /// <param name="name">
        ///     A custom name for the dbcontext to get a named dbcontext.
        ///     If there is no dbcontext in this unit of work with given name, then a new one is created.
        /// </param>
        public static Task<TDbContext> GetDbContextAsync<TDbContext>(this IActiveUnitOfWork unitOfWork,
            MultiCompanySides? multiCompanySide = null, string name = null)
            where TDbContext : DbContext
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (!(unitOfWork is EfCoreUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(EfCoreUnitOfWork).FullName,
                    nameof(unitOfWork));
            }

            return (unitOfWork as EfCoreUnitOfWork).GetOrCreateDbContextAsync<TDbContext>(multiCompanySide, name);
        }

        public static TDbContext GetDbContext<TDbContext>(this IActiveUnitOfWork unitOfWork,
            MultiCompanySides? multiCompanySide = null, string name = null)
            where TDbContext : DbContext
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (!(unitOfWork is EfCoreUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(EfCoreUnitOfWork).FullName,
                    nameof(unitOfWork));
            }

            return (unitOfWork as EfCoreUnitOfWork).GetOrCreateDbContext<TDbContext>(multiCompanySide, name);
        }
    }
}
