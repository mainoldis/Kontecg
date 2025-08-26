using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Collections.Extensions;

namespace Kontecg.Modules
{
    /// <summary>
    ///     Used to store KontecgModuleInfo objects as a dictionary.
    /// </summary>
    internal class KontecgModuleCollection : List<KontecgModuleInfo>
    {
        public KontecgModuleCollection(Type startupModuleType)
        {
            StartupModuleType = startupModuleType;
        }

        public Type StartupModuleType { get; }

        /// <summary>
        ///     Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Module type</typeparam>
        /// <returns>Reference to the module instance</returns>
        public TModule GetModule<TModule>() where TModule : KontecgModule
        {
            KontecgModuleInfo module = this.FirstOrDefault(m => m.Type == typeof(TModule));
            if (module == null)
            {
                throw new KontecgException("Can not find module for " + typeof(TModule).FullName);
            }

            return (TModule) module.Instance;
        }

        /// <summary>
        ///     Sorts modules according to dependencies.
        ///     If module A depends on module B, A comes after B in the returned List.
        /// </summary>
        /// <returns>Sorted list</returns>
        public List<KontecgModuleInfo> GetSortedModuleListByDependency()
        {
            List<KontecgModuleInfo> sortedModules = this.SortByDependencies(x => x.Dependencies);
            EnsureKernelModuleToBeFirst(sortedModules);
            EnsureStartupModuleToBeLast(sortedModules, StartupModuleType);
            return sortedModules;
        }

        public static void EnsureKernelModuleToBeFirst(List<KontecgModuleInfo> modules)
        {
            int kernelModuleIndex = modules.FindIndex(m => m.Type == typeof(KontecgKernelModule));
            if (kernelModuleIndex <= 0)
                //It's already the first!
            {
                return;
            }

            KontecgModuleInfo kernelModule = modules[kernelModuleIndex];
            modules.RemoveAt(kernelModuleIndex);
            modules.Insert(0, kernelModule);
        }

        public static void EnsureStartupModuleToBeLast(List<KontecgModuleInfo> modules, Type startupModuleType)
        {
            int startupModuleIndex = modules.FindIndex(m => m.Type == startupModuleType);
            if (startupModuleIndex >= modules.Count - 1)
                //It's already the last!
            {
                return;
            }

            KontecgModuleInfo startupModule = modules[startupModuleIndex];
            modules.RemoveAt(startupModuleIndex);
            modules.Add(startupModule);
        }

        public void EnsureKernelModuleToBeFirst()
        {
            EnsureKernelModuleToBeFirst(this);
        }

        public void EnsureStartupModuleToBeLast()
        {
            EnsureStartupModuleToBeLast(this, StartupModuleType);
        }
    }
}
