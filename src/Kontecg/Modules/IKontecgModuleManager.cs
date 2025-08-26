using System;
using System.Collections.Generic;

namespace Kontecg.Modules
{
    public interface IKontecgModuleManager
    {
        KontecgModuleInfo StartupModule { get; }

        IReadOnlyList<KontecgModuleInfo> Modules { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}
