using System;

namespace Kontecg.Baseline.Configuration
{
    public interface IKontecgBaselineEntityTypes
    {
        /// <summary>
        ///     User type of the application.
        /// </summary>
        Type User { get; set; }

        /// <summary>
        ///     Role type of the application.
        /// </summary>
        Type Role { get; set; }

        /// <summary>
        ///     Company type of the application.
        /// </summary>
        Type Company { get; set; }
    }
}
