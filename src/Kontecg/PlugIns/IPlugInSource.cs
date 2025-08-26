using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kontecg.PlugIns
{
    public interface IPlugInSource
    {
        List<Assembly> GetAssemblies();

        List<Type> GetModules();
    }
}
