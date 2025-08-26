using Kontecg.BlobStoring;

namespace Kontecg.BlobStoring.FileSystem
{
    public interface IBlobFilePathCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}