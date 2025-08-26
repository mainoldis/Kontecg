using System;
using System.Data;
using System.Threading.Tasks;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal class NotSupportedAsyncAdapter : IAsyncAdapter
    {
        public Task<int> ExecuteNonQueryAsync(IDbCommand command)
        {
            throw new NotSupportedException(
                "Async is not supported or not configured for this provider. Enable async support by setting the IAsyncAdapter via Db.Configure().");
        }

        public Task<object> ExecuteScalarAsync(IDbCommand command)
        {
            throw new NotSupportedException(
                "Async is not supported or not configured for this provider. Enable async support by setting the IAsyncAdapter via Db.Configure().");
        }

        public Task<IDataReader> ExecuteReaderAsync(IDbCommand command)
        {
            throw new NotSupportedException(
                "Async is not supported or not configured for this provider. Enable async support by setting the IAsyncAdapter via Db.Configure().");
        }

        public Task OpenConnectionAsync(IDbConnection connection)
        {
            throw new NotSupportedException();
        }
    }
}
