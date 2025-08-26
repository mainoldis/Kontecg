using System;
using System.Data;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal class DbConfigurationBuilder : IDbConfigurationBuilder
    {
        private readonly DbConfig _dbConfig;

        internal DbConfigurationBuilder(DbConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public IDbConfigurationBuilder SetAsyncAdapter(IAsyncAdapter asyncAdapter)
        {
            _dbConfig.AsyncAdapter = asyncAdapter;
            return this;
        }

        public IDbConfigurationBuilder OnPrepareCommand(Action<IDbCommand> action)
        {
            _dbConfig.PrepareCommand = action;
            return this;
        }

        public DbConfigurationBuilder Default()
        {
            SetAsyncAdapter(new NotSupportedAsyncAdapter());
            OnPrepareCommand(a => { });
            return this;
        }

        public DbConfigurationBuilder SqlServer()
        {
            SetAsyncAdapter(new SqlAsyncAdapter());
            OnPrepareCommand(a => { });
            return this;
        }

        public DbConfigurationBuilder Oracle()
        {
            SetAsyncAdapter(new NotSupportedAsyncAdapter());
            OnPrepareCommand(command =>
            {
                dynamic c = command;
                c.BindByName = true;
            });
            return this;
        }

        public DbConfigurationBuilder FromProviderName(string providerName)
        {
            switch (providerName)
            {
                case "Oracle.DataAccess.Client":
                    return Oracle();
                case "System.Data.SqlClient":
                    return SqlServer();
                default:
                    return Default();
            }
        }
    }
}
