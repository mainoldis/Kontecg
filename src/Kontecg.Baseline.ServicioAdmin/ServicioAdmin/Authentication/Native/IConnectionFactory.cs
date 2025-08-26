using System.Data;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal interface IConnectionFactory
    {
        /// <summary>
        ///     Create the ADO.Net IDbConnection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>the connection</returns>
        IDbConnection CreateConnection(string connectionString);
    }
}
