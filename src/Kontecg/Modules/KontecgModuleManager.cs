using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.PlugIns;

namespace Kontecg.Modules
{
    /// <summary>
    ///     This class is used to manage modules.
    /// </summary>
    public class KontecgModuleManager : IKontecgModuleManager
    {
        private readonly IIocManager _iocManager;
        private readonly IKontecgPlugInManager _kontecgPlugInManager;

        private KontecgModuleCollection _modules;

        public KontecgModuleManager(IIocManager iocManager, IKontecgPlugInManager kontecgPlugInManager)
        {
            _iocManager = iocManager;
            _kontecgPlugInManager = kontecgPlugInManager;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public KontecgModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<KontecgModuleInfo> Modules => _modules.ToImmutableList();

        public virtual void Initialize(Type startupModule)
        {
            _modules = new KontecgModuleCollection(startupModule);
            LoadAllModules();
        }

        public virtual void StartModules()
        {
            List<KontecgModuleInfo> sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        public virtual void ShutdownModules()
        {
            Logger.Debug("Shutting down has been started");

            List<KontecgModuleInfo> sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            Logger.Debug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            Logger.Debug("Loading Kontecg modules...");

            List<Type> moduleTypes = FindAllModuleTypes(out List<Type> plugInModuleTypes).Distinct().ToList();

            Logger.Debug("Found " + moduleTypes.Count + " Kontecg modules in total.");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes, plugInModuleTypes);

            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", _modules.Count);
        }

        private List<Type> FindAllModuleTypes(out List<Type> plugInModuleTypes)
        {
            plugInModuleTypes = new List<Type>();

            List<Type> modules =
                KontecgModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(_modules.StartupModuleType);

            foreach (Type plugInModuleType in _kontecgPlugInManager.PlugInSources.GetAllModules())
            {
                if (modules.AddIfNotContains(plugInModuleType))
                {
                    plugInModuleTypes.Add(plugInModuleType);
                }
            }

            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes, List<Type> plugInModuleTypes)
        {
            foreach (Type moduleType in moduleTypes)
            {
                if (!(_iocManager.Resolve(moduleType) is KontecgModule moduleObject))
                {
                    throw new KontecgInitializationException("This type is not an Kontecg module: " +
                                                             moduleType.AssemblyQualifiedName);
                }

                moduleObject.IocManager = _iocManager;
                moduleObject.Configuration = _iocManager.Resolve<IKontecgStartupConfiguration>();

                KontecgModuleInfo moduleInfo = new(moduleType, moduleObject, plugInModuleTypes.Contains(moduleType));

                _modules.Add(moduleInfo);

                if (moduleType == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

                Logger.DebugFormat("Loaded module: " + moduleInfo.Name);
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (Type moduleType in moduleTypes)
            {
                _iocManager.RegisterIfNot(moduleType);
            }
        }

        private void SetDependencies()
        {
            foreach (KontecgModuleInfo moduleInfo in _modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (Type dependedModuleType in KontecgModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    KontecgModuleInfo dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new KontecgInitializationException("Could not find a depended module " +
                                                                 dependedModuleType.AssemblyQualifiedName + " for " +
                                                                 moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if (moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null)
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
