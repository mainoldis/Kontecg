﻿using System.Configuration;

namespace Kontecg.Data
{
    public static class ConnectionStringHelper
    {
        /// <summary>
        ///     Gets connection string from given connection string or name.
        /// </summary>
        public static string GetConnectionString(string nameOrConnectionString)
        {
            ConnectionStringSettings connStrSection = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (connStrSection != null)
            {
                return connStrSection.ConnectionString;
            }

            return nameOrConnectionString;
        }
    }
}
