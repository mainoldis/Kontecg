using System;
using System.Data;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal interface IDb : IDisposable
    {
        /// <summary>
        ///     The actual IDbConnection (which will be open)
        /// </summary>
        IDbConnection Connection { get; }

        string ConnectionString { get; }

        /// <summary>
        ///     Entry point for configuring the db with provider-specific stuff.
        ///     Specifically, allows to set the async adapter
        /// </summary>
        /// <returns></returns>
        IDbConfigurationBuilder Configure();

        /// <summary>
        ///     Create a SQL query command builder
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns>a CommandBuilder instance</returns>
        CommandBuilder Sql(string sqlQuery);

        /// <summary>
        ///     Create a Stored Procedure command
        /// </summary>
        /// <param name="sprocName">name of the sproc</param>
        /// <returns>a CommandBuilder instance</returns>
        CommandBuilder StoredProcedure(string sprocName);

        /// <summary>
        ///     Create a SQL command and execute it immediately (non query)
        /// </summary>
        /// <param name="command"></param>
        int Execute(string command);
    }
}
