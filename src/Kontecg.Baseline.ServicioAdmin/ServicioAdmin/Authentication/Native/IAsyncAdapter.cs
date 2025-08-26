using System.Data;
using System.Threading.Tasks;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal interface IAsyncAdapter
    {
        Task<int> ExecuteNonQueryAsync(IDbCommand command);
        Task<object> ExecuteScalarAsync(IDbCommand command);
        Task<IDataReader> ExecuteReaderAsync(IDbCommand command);
        Task OpenConnectionAsync(IDbConnection connection);
    }
}
