using System;
using System.Data;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal class DbConfig
    {
        public Action<IDbCommand> PrepareCommand { get; internal set; }
        public IAsyncAdapter AsyncAdapter { get; internal set; }
    }
}
