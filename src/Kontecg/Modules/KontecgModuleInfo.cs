using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Kontecg.Modules
{
    /// <summary>
    ///     Used to store all needed information for a module.
    /// </summary>
    public class KontecgModuleInfo
    {
        /// <summary>
        ///     Creates a new KontecgModuleInfo object.
        /// </summary>
        public KontecgModuleInfo([NotNull] Type type, [NotNull] KontecgModule instance, bool isLoadedAsPlugIn)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            Type = type;
            Instance = instance;
            IsLoadedAsPlugIn = isLoadedAsPlugIn;
            Assembly = Type.GetTypeInfo().Assembly;

            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(Assembly.Location);
            Name = Type.GetTypeInfo().Name;
            Description = fileInfo.FileDescription;
            Version = Version.TryParse(fileInfo.FileVersion, out Version v)
                ? v
                : new Version(1, 0, 0, 0);

            Dependencies = new List<KontecgModuleInfo>();
        }

        public string Name { get; }

        public string Description { get; }

        public Version Version { get; }

        /// <summary>
        ///     The assembly which contains the module definition.
        /// </summary>
        [JsonIgnore]
        public Assembly Assembly { get; }

        /// <summary>
        ///     Type of the module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Instance of the module.
        /// </summary>
        [JsonIgnore]
        public KontecgModule Instance { get; }

        /// <summary>
        ///     Is this module loaded as a plugin.
        /// </summary>
        public bool IsLoadedAsPlugIn { get; }

        /// <summary>
        ///     All dependent modules of this module.
        /// </summary>
        [JsonIgnore]
        public List<KontecgModuleInfo> Dependencies { get; }

        public override string ToString()
        {
            return Type.AssemblyQualifiedName ??
                   Type.FullName;
        }
    }
}
