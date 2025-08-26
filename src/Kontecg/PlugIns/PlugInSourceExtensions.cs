using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Modules;

namespace Kontecg.PlugIns
{
    public static class PlugInSourceExtensions
    {
        public static List<Type> GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(KontecgModule.FindDependedModuleTypesRecursivelyIncludingGivenModule)
                .Distinct()
                .ToList();
        }
    }
}
