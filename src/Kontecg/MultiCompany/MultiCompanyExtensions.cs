using Kontecg.Domain.Entities;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Extension methods for multi-company.
    /// </summary>
    public static class MultiCompanyExtensions
    {
        /// <summary>
        ///     Gets multi-company side (<see cref="MultiCompanySides" />) of an object that implements
        ///     <see cref="IMayHaveCompany" />.
        /// </summary>
        /// <param name="obj">The object</param>
        public static MultiCompanySides GetMultiCompanySide(this IMayHaveCompany obj)
        {
            return obj.CompanyId.HasValue
                ? MultiCompanySides.Company
                : MultiCompanySides.Host;
        }
    }
}
