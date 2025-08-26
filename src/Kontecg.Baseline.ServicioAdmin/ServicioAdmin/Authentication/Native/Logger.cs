using System;
using System.Diagnostics;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal class Logger
    {
        public static Action<string> Log = s => Trace.WriteLine(s);
    }
}
