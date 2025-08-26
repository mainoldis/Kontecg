using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontecg.Modules;

namespace Kontecg.Reflection
{
    public class KontecgAssemblyFinder : IAssemblyFinder
    {
        private readonly IKontecgModuleManager _moduleManager;

        public KontecgAssemblyFinder(IKontecgModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();

            foreach (KontecgModuleInfo module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            return assemblies.Distinct().ToList();
        }
    }
}
