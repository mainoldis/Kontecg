using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Kontecg.Extensions;
using Kontecg.IO.Extensions;

namespace Kontecg.Resources.Embedded
{
    public class EmbeddedResourceSet
    {
        public EmbeddedResourceSet(string rootPath, Assembly assembly, string resourceNamespace)
        {
            RootPath = rootPath.EnsureEndsWith('/');
            Assembly = assembly;
            ResourceNamespace = resourceNamespace;
        }

        public string RootPath { get; }

        public Assembly Assembly { get; }

        public string ResourceNamespace { get; }

        internal void AddResources(Dictionary<string, EmbeddedResourceItem> resources)
        {
            foreach (string resourceName in Assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith(ResourceNamespace))
                {
                    continue;
                }

                using Stream stream = Assembly.GetManifestResourceStream(resourceName);
                string relativePath = ConvertToRelativePath(resourceName);
                string filePath = EmbeddedResourcePathHelper.NormalizePath(RootPath) + relativePath;

                resources[filePath] = new EmbeddedResourceItem(
                    filePath,
                    stream.GetAllBytes(),
                    Assembly
                );
            }
        }

        private string ConvertToRelativePath(string resourceName)
        {
            return resourceName.Substring(ResourceNamespace.Length + 1);
        }
    }
}
