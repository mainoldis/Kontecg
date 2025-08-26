using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontecg.Collections.Extensions;
using Kontecg.Modules;

namespace Kontecg.PlugIns
{
    //TODO: This class is similar to FolderPlugInSource. Create an abstract base class for them.
    public class AssemblyFileListPlugInSource : IPlugInSource
    {
        private readonly Lazy<List<Assembly>> _assemblies;

        public AssemblyFileListPlugInSource(params string[] filePaths)
        {
            FilePaths = filePaths ?? new string[0];

            _assemblies = new Lazy<List<Assembly>>(LoadAssemblies, true);
        }

        public string[] FilePaths { get; }

        public List<Assembly> GetAssemblies()
        {
            return _assemblies.Value;
        }

        public List<Type> GetModules()
        {
            List<Type> modules = new List<Type>();

            foreach (Assembly assembly in GetAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (KontecgModule.IsKontecgModule(type))
                        {
                            modules.AddIfNotContains(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new KontecgInitializationException(
                        "Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules;
        }

        private List<Assembly> LoadAssemblies()
        {
            return FilePaths.Select(
                Assembly.LoadFile //TODO: Use AssemblyLoadContext.Default.LoadFromAssemblyPath instead?
            ).ToList();
        }
    }
}
