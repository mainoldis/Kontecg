using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal class CommandBuilder
    {
        public CommandBuilder(IDbCommand command) : this(command, null)
        {
        }

        public CommandBuilder(IDbCommand command, IAsyncAdapter asyncAdapter)
        {
            Command = command;
            AsyncAdapter = asyncAdapter;
        }

        /// <summary>
        ///     The raw IDbCommand instance
        /// </summary>
        public IDbCommand Command { get; }

        private IAsyncAdapter AsyncAdapter { get; }

        /// <summary>
        ///     executes the query and returns the result as a list of dynamic objects
        /// </summary>
        /// <returns></returns>
        public IEnumerable<dynamic> AsEnumerable()
        {
            return Execute().Reader().AsEnumerable().ToDynamic();
        }

        /// <summary>
        ///     executes the query and returns the result as a list of lists
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEnumerable<dynamic>> AsMultiResultSet()
        {
            using (IDataReader reader = Execute().Reader())
            {
                return reader.ToMultiResultSet();
            }
        }

        /// <summary>
        ///     Executes the command, returning the first column of the first result, converted to the type T
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <returns></returns>
        public T AsScalar<T>()
        {
            object result = Execute().Scalar();
            return ConvertTo<T>.From(result);
        }

        /// <summary>
        ///     Executes the command as a SQL statement, not returning any results
        /// </summary>
        public int AsNonQuery()
        {
            return Execute().NonQuery();
        }

        private Executor Execute()
        {
            Log();
            return new Executor(Command);
        }

        public async Task<int> AsNonQueryAsync()
        {
            await PrepareAsync();
            return await AsyncAdapter.ExecuteNonQueryAsync(Command);
        }

        public async Task<T> AsScalarAsync<T>()
        {
            await PrepareAsync();
            object result = await AsyncAdapter.ExecuteScalarAsync(Command);
            return ConvertTo<T>.From(result);
        }

        public async Task<IEnumerable<dynamic>> AsEnumerableAsync()
        {
            await PrepareAsync();
            IDataReader reader = await AsyncAdapter.ExecuteReaderAsync(Command);
            return reader.AsEnumerable().ToDynamic();
        }

        public async Task<IEnumerable<IEnumerable<dynamic>>> AsMultiResultSetAsync()
        {
            await PrepareAsync();
            using (IDataReader reader = await AsyncAdapter.ExecuteReaderAsync(Command))
            {
                return reader.ToMultiResultSet();
            }
        }

        private async Task PrepareAsync()
        {
            Log();
            await AsyncAdapter.OpenConnectionAsync(Command.Connection);
        }

        private void Log()
        {
            Logger.Log(Command.CommandText);
            if (Command.Parameters != null)
            {
                foreach (IDbDataParameter p in Command.Parameters)
                {
                    Logger.Log(string.Format("{0} = {1}", p.ParameterName, p.Value));
                }
            }
        }

        public CommandBuilder WithCommandText(string text)
        {
            Command.CommandText = text;
            return this;
        }

        public CommandBuilder OfType(CommandType type)
        {
            Command.CommandType = type;
            return this;
        }

        public CommandBuilder WithParameters(dynamic parameters)
        {
            object o = parameters;
            PropertyInfo[] props = o.GetType().GetProperties();
            foreach (PropertyInfo item in props)
            {
                WithParameter(item.Name, item.GetValue(o, null));
            }

            return this;
        }

        /// <summary>
        ///     Builder method - sets the command timeout
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public CommandBuilder WithTimeout(TimeSpan timeout)
        {
            Command.CommandTimeout = (int) timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        ///     Builder method - adds a name/value pair as parameter
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="value">the parameter value</param>
        /// <returns>the same CommandBuilder instance</returns>
        public CommandBuilder WithParameter(string name, object value)
        {
            IDbDataParameter p = Command.CreateParameter();
            p.ParameterName = name;
            p.Value = DBNullHelper.ToDb(value);
            Command.Parameters.Add(p);
            return this;
        }

        /// <summary>
        ///     Builder method - adds a table-valued parameter. Only supported on SQL Server (System.Data.SqlClient)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">parameter name</param>
        /// <param name="values">list of values</param>
        /// <param name="udtTypeName">name of the user-defined table type</param>
        /// <returns></returns>
        public CommandBuilder WithParameter<T>(string name, IEnumerable<T> values, string udtTypeName)
        {
            DataTable dataTable = values.ToDataTable();

            SqlParameter p = new(name, SqlDbType.Structured)
            {
                TypeName = udtTypeName,
                Value = dataTable
            };

            Command.Parameters.Add(p);
            return this;
        }
    }
}
