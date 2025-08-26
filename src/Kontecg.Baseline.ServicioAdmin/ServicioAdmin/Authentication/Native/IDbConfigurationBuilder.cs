using System;
using System.Data;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal interface IDbConfigurationBuilder
    {
        IDbConfigurationBuilder SetAsyncAdapter(IAsyncAdapter asyncAdapter);
        IDbConfigurationBuilder OnPrepareCommand(Action<IDbCommand> action);
    }
}
