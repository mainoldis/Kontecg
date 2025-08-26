namespace Kontecg.BlobStoring
{
    public class KontecgBlobStoringOptions
    {
        public BlobContainerConfigurations Containers { get; }

        public KontecgBlobStoringOptions()
        {
            Containers = new BlobContainerConfigurations();
        }
    }
}
