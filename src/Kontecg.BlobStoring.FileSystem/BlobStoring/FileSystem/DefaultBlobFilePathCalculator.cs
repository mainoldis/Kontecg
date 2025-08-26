using System.IO;
using Kontecg.Dependency;
using Kontecg.Runtime.Session;

namespace Kontecg.BlobStoring.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator, ITransientDependency
    {
        protected IKontecgSession KontecgSession { get; }

        public DefaultBlobFilePathCalculator(IKontecgSession session)
        {
            KontecgSession = session;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var blobPath = fileSystemConfiguration.BasePath;

            blobPath = KontecgSession.CompanyId == null
                ? Path.Combine(blobPath, "host")
                : Path.Combine(blobPath, "companies", KontecgSession.CompanyId.Value.ToString("D"));

            if (fileSystemConfiguration.AppendContainerNameToBasePath)
            {
                blobPath = Path.Combine(blobPath, args.ContainerName);
            }

            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}
