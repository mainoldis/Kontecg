using Kontecg.Domain.Entities;

namespace Kontecg.Domain.Uow
{
    /// <summary>
    /// Standard filters of KONTECG.
    /// </summary>
    public static class KontecgDataFilters
    {
        /// <summary>
        /// "SoftDelete".
        /// Soft delete filter.
        /// Prevents getting deleted data from database.
        /// See <see cref="ISoftDelete"/> interface.
        /// </summary>
        public const string SoftDelete = "SoftDelete";

        /// <summary>
        /// "MustHaveCompany".
        /// Company filter to prevent getting data that is
        /// not belong to current company.
        /// </summary>
        public const string MustHaveCompany = "MustHaveCompany";

        /// <summary>
        /// "MayHaveCompany".
        /// Company filter to prevent getting data that is
        /// not belong to current company.
        /// </summary>
        public const string MayHaveCompany = "MayHaveCompany";

        /// <summary>
        /// Standard parameters of KONTECG.
        /// </summary>
        public static class Parameters
        {
            /// <summary>
            /// "companyId".
            /// </summary>
            public const string CompanyId = "companyId";

            /// <summary>
            /// "isDeleted".
            /// </summary>
            public const string IsDeleted = "isDeleted";
        }
    }

    /// <summary>
    /// Standard filters of KONTECG.
    /// </summary>
    public static class KontecgAuditFields
    {
        public const string CreatorUserId = "CreatorUserId";

        public const string LastModifierUserId = "LastModifierUserId";

        public const string DeleterUserId = "DeleterUserId";

        public const string LastModificationTime = "LastModificationTime";

        public const string DeletionTime = "DeletionTime";
    }
}
