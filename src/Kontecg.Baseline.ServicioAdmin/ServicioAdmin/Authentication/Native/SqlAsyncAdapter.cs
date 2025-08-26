using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    public class SqlAsyncAdapter : IAsyncAdapter
    {
        public async Task<int> ExecuteNonQueryAsync(IDbCommand command)
        {
            SqlCommand sqlCommand = (SqlCommand) command;
            int result = await sqlCommand.ExecuteNonQueryAsync(CancellationToken.None);
            return result;
        }

        public async Task<object> ExecuteScalarAsync(IDbCommand command)
        {
            SqlCommand sqlCommand = (SqlCommand) command;
            object result = await sqlCommand.ExecuteScalarAsync();
            return result;
        }

        public async Task<IDataReader> ExecuteReaderAsync(IDbCommand command)
        {
            SqlCommand sqlCommand = (SqlCommand) command;
            SqlDataReader result = await sqlCommand.ExecuteReaderAsync();
            return result;
        }

        public async Task OpenConnectionAsync(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                SqlConnection sqlConnection = (SqlConnection) connection;
                await sqlConnection.OpenAsync();
            }
        }
    }
}
