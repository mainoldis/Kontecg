// Yet Another Data Access Layer
// usage:
//   using (var db = new Db()) {};
//   using (var db = Db.FromConfig());
// from there it should be discoverable.
// inline SQL FTW!

using System;
using System.Configuration;
using System.Data;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    /// <summary>
    ///     A class that wraps a database.
    /// </summary>
    internal class Db : IDb
    {
        /// <summary>
        ///     The default DbProvider name is "System.Data.SqlClient" (for sql server).
        /// </summary>
        public static string DefaultProviderName = "System.Data.SqlClient";

        private readonly DbConfig _config = new();
        private readonly IConnectionFactory _connectionFactory;

        private readonly IDbConnection _externalConnection;
        private Lazy<IDbConnection> _connection;

        /// <summary>
        ///     Instantiate Db with existing connection. The connection is only used for creating commands; it should be disposed
        ///     by the caller when done.
        /// </summary>
        /// <param name="connection">The existing connection</param>
        /// <param name="providerName"></param>
        public Db(IDbConnection connection, string providerName = null)
        {
            _externalConnection = connection;
            ConfigurePriv().FromProviderName(providerName);
        }

        /// <summary>
        ///     Instantiate Db with connectionString and DbProviderName
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="providerName">
        ///     The ADO .Net Provider name. When not specified, the default value is used (see
        ///     DefaultProviderName)
        /// </param>
        public Db(string connectionString, string providerName = null)
            : this(connectionString, new AdoNetProviderFactory(providerName ?? DefaultProviderName), providerName)
        {
        }

        /// <summary>
        ///     Instantiate Db with connectionString and a custom IConnectionFactory
        /// </summary>
        /// <param name="connectionString">the connection string</param>
        /// <param name="connectionFactory">the connection factory</param>
        /// <param name="providerName"></param>
        public Db(string connectionString, IConnectionFactory connectionFactory, string providerName = null)
        {
            ConnectionString = connectionString;
            _connectionFactory = connectionFactory;
            _connection = new Lazy<IDbConnection>(CreateConnection);
            string providerInvariantName = providerName ?? DefaultProviderName;
            ConfigurePriv().FromProviderName(providerInvariantName);
        }

        public IDbConfigurationBuilder Configure()
        {
            return ConfigurePriv();
        }

        /// <summary>
        ///     The actual IDbConnection (which will be open)
        /// </summary>
        public IDbConnection Connection => _externalConnection ?? _connection.Value;

        public string ConnectionString { get; }

        public void Dispose()
        {
            if (_connection == null || !_connection.IsValueCreated)
            {
                return;
            }

            _connection.Value.Dispose();
            _connection = null;
        }

        /// <summary>
        ///     Create a SQL query command builder
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns>a CommandBuilder instance</returns>
        public CommandBuilder Sql(string sqlQuery)
        {
            return CreateCommand(CommandType.Text, sqlQuery);
        }

        /// <summary>
        ///     Create a Stored Procedure command
        /// </summary>
        /// <param name="sprocName">name of the sproc</param>
        /// <returns>a CommandBuilder instance</returns>
        public CommandBuilder StoredProcedure(string sprocName)
        {
            return CreateCommand(CommandType.StoredProcedure, sprocName);
        }

        /// <summary>
        ///     Create a SQL command and execute it immediately (non query)
        /// </summary>
        /// <param name="command"></param>
        public int Execute(string command)
        {
            return Sql(command).AsNonQuery();
        }

        private DbConfigurationBuilder ConfigurePriv()
        {
            return new DbConfigurationBuilder(_config);
        }

        /// <summary>
        ///     Factory method, instantiating the Db class from the first connectionstring in the app.config or web.config file.
        /// </summary>
        /// <returns>Db</returns>
        public static Db FromConfig()
        {
            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;
            ConnectionStringSettings connectionStringSettings = connectionStrings[0];
            return FromConfig(connectionStringSettings);
        }

        /// <summary>
        ///     Factory method, instantiating the Db class from a named connectionstring in the app.config or web.config file.
        /// </summary>
        public static Db FromConfig(string connectionStringName)
        {
            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;
            return FromConfig(connectionStrings[connectionStringName]);
        }

        private static Db FromConfig(ConnectionStringSettings connectionStringSettings)
        {
            string connectionString = connectionStringSettings.ConnectionString;
            string providerName = !string.IsNullOrEmpty(connectionStringSettings.ProviderName)
                ? connectionStringSettings.ProviderName
                : DefaultProviderName;
            return new Db(connectionString, providerName);
        }

        private IDbConnection CreateConnection()
        {
            IDbConnection connection = _connectionFactory.CreateConnection(ConnectionString);
            return connection;
        }

        private CommandBuilder CreateCommand(CommandType commandType, string command)
        {
            IDbCommand cmd = Connection.CreateCommand();
            _config.PrepareCommand(cmd);
            return new CommandBuilder(cmd, _config.AsyncAdapter).OfType(commandType).WithCommandText(command);
        }
    }
}
