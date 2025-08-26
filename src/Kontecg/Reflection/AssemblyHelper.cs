using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kontecg.Reflection
{
    internal static class AssemblyHelper
    {
        public static List<Assembly> GetAllAssembliesInFolder(string folderPath, SearchOption searchOption)
        {
            IEnumerable<string> assemblyFiles = Directory
                .EnumerateFiles(folderPath, "*.*", searchOption)
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));

            return assemblyFiles.Select(
                Assembly.LoadFile
            ).ToList();
        }
    }
}
