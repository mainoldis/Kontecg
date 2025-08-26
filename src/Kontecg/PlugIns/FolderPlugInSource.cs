using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Kontecg.Collections.Extensions;
using Kontecg.Modules;
using Kontecg.Reflection;

namespace Kontecg.PlugIns
{
    public class FolderPlugInSource : IPlugInSource
    {
        private readonly Lazy<List<Assembly>> _assemblies;

        public FolderPlugInSource(string folder, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Folder = folder;
            SearchOption = searchOption;

            _assemblies = new Lazy<List<Assembly>>(LoadAssemblies, true);
        }

        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

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
            return AssemblyHelper.GetAllAssembliesInFolder(Folder, SearchOption);
        }
    }
}
