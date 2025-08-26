namespace Kontecg
{
    /// <summary>
    ///     Used to define some constants for Kontecg.
    /// </summary>
    public static class KontecgConsts
    {
        /// <summary>
        ///     Localization source name of Kontecg framework.
        /// </summary>
        public const string LocalizationSourceName = "Kontecg";

        internal static class DefaultUpdateSources
        {
            public const string LocalUpdateSourcePath = "Updates";
        }

        internal static class Orms
        {
            public const string Dapper = "Dapper";
            public const string EntityFrameworkCore = "EntityFrameworkCore";
            public const string NHibernate = "NHibernate";
        }
    }
}
