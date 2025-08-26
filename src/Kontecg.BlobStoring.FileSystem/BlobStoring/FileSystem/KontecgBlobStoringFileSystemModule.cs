using System.Reflection;
using Kontecg.Modules;

namespace Kontecg.BlobStoring.FileSystem
{
    [DependsOn(typeof(KontecgBlobStoringModule))]
    public class KontecgBlobStoringFileSystemModule : KontecgModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
