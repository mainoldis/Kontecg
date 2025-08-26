using System;

namespace Kontecg.Events.Bus
{
    public interface IEventDataMayHaveCompanyId
    {
        /// <summary>
        /// Returns true if this event data has a Company Id information.
        /// If so, it should set the <paramref name="companyId"/> our parameter.
        /// Otherwise, the <paramref name="companyId"/> our parameter value should not be informative
        /// (it will be null as expected, but doesn't indicate a company with null id).
        /// </summary>
        /// <param name="companyId">
        ///     The company id that is set if this method returns true.
        /// </param>
        bool IsMultiCompany(out int? companyId);
    }
}
