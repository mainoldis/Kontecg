using System;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Represents sides in a multi tenancy application.
    /// </summary>
    [Flags]
    public enum MultiCompanySides
    {
        /// <summary>
        ///     Company side.
        /// </summary>
        Company = 1,

        /// <summary>
        ///     Host (tenancy owner) side.
        /// </summary>
        Host = 2
    }
}
