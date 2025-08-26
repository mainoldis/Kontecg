using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;

namespace Kontecg.Modules
{
    /// <summary>
    ///     This class must be implemented by all module definition classes.
    /// </summary>
    /// <remarks>
    ///     A module definition class is generally located in its own assembly
    ///     and implements some action in module events on application startup and shutdown.
    ///     It also defines depended modules.
    /// </remarks>
    public abstract class KontecgModule
    {
        protected KontecgModule()
        {
            Logger = NullLogger.Instance;
        }

        /// <summary>
        ///     Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///     Gets a reference to the IOC manager.
        /// </summary>
        protected internal IIocManager IocManager { get; internal set; }

        /// <summary>
        ///     Gets a reference to the Kontecg configuration.
        /// </summary>
        protected internal IKontecgStartupConfiguration Configuration { get; internal set; }

        /// <summary>
        ///     This is the first event called on application startup.
        ///     Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public virtual void PreInitialize()
        {
        }

        /// <summary>
        ///     This method is used to register dependencies for this module.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        ///     This method is called lastly on application startup.
        /// </summary>
        public virtual void PostInitialize()
        {
        }

        /// <summary>
        ///     This method is called when the application is being shutdown.
        /// </summary>
        public virtual void Shutdown()
        {
        }

        public virtual Assembly[] GetAdditionalAssemblies()
        {
            return Array.Empty<Assembly>();
        }

        /// <summary>
        ///     Checks if given type is an Kontecg module class.
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsKontecgModule(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(KontecgModule).IsAssignableFrom(type);
        }

        /// <summary>
        ///     Finds direct depended modules of a module (excluding given module).
        /// </summary>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsKontecgModule(moduleType))
            {
                throw new KontecgInitializationException("This type is not an Kontecg module: " +
                                                         moduleType.AssemblyQualifiedName);
            }

            List<Type> list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(typeof(DependsOnAttribute), true))
            {
                IEnumerable<DependsOnAttribute> dependsOnAttributes = moduleType.GetTypeInfo()
                    .GetCustomAttributes(typeof(DependsOnAttribute), true)
                    .Cast<DependsOnAttribute>();
                foreach (DependsOnAttribute dependsOnAttribute in dependsOnAttributes)
                foreach (Type dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                {
                    list.Add(dependedModuleType);
                }
            }

            return list;
        }

        public static List<Type> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            List<Type> list = new List<Type>();
            AddModuleAndDependenciesRecursively(list, moduleType);
            list.AddIfNotContains(typeof(KontecgKernelModule));
            return list;
        }

        private static void AddModuleAndDependenciesRecursively(List<Type> modules, Type module)
        {
            if (!IsKontecgModule(module))
            {
                throw new KontecgInitializationException("This type is not an Kontecg module: " +
                                                         module.AssemblyQualifiedName);
            }

            if (modules.Contains(module))
            {
                return;
            }

            modules.Add(module);

            List<Type> dependedModules = FindDependedModuleTypes(module);
            foreach (Type dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(modules, dependedModule);
            }
        }
    }
}
