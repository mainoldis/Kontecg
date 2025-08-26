using System;
using System.IO;
using System.Reflection;
using Kontecg.Dependency;

namespace Kontecg.Updates
{
    public class KontecgLocalSource : IUpdateSource
    {
        public KontecgLocalSource(Uri source = null)
        {
            Source = source;
        }

        public string Name => nameof(KontecgLocalSource);

        public Uri Source { get; private set; }

        public void Initialize(IUpdateConfiguration configuration, IIocResolver iocResolver)
        {
            Source = Source ??
                     new Uri(Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\",
                         "packages")));
        }
    }
}
