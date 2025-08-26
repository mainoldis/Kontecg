using System.Data;
using System.Data.Common;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal class AdoNetProviderFactory : IConnectionFactory
    {
        private readonly string _providerInvariantName;

        public AdoNetProviderFactory(string providerInvariantName)
        {
            _providerInvariantName = providerInvariantName;
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            DbConnection connection = DbProviderFactories.GetFactory(_providerInvariantName).CreateConnection();
            // ReSharper disable once PossibleNullReferenceException
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
