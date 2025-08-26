using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Kontecg.Collections.Extensions;

namespace Kontecg.Reflection
{
    public class TypeFinder : ITypeFinder
    {
        private readonly IAssemblyFinder _assemblyFinder;
        private readonly object _syncObj = new();
        private Type[] _types;

        public TypeFinder(IAssemblyFinder assemblyFinder)
        {
            _assemblyFinder = assemblyFinder;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public Type[] Find(Func<Type, bool> predicate)
        {
            return GetAllTypes().Where(predicate).ToArray();
        }

        public Type[] FindAll()
        {
            return GetAllTypes().ToArray();
        }

        private Type[] GetAllTypes()
        {
            if (_types == null)
            {
                lock (_syncObj)
                {
                    _types ??= CreateTypeList().ToArray();
                }
            }

            return _types;
        }

        private List<Type> CreateTypeList()
        {
            List<Type> allTypes = new List<Type>();

            IEnumerable<Assembly> assemblies = _assemblyFinder.GetAllAssemblies().Distinct();

            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Type[] typesInThisAssembly;

                    try
                    {
                        typesInThisAssembly = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        typesInThisAssembly = ex.Types;
                    }

                    if (typesInThisAssembly.IsNullOrEmpty())
                    {
                        continue;
                    }

                    allTypes.AddRange(typesInThisAssembly.Where(type => type != null));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            return allTypes;
        }
    }
}
