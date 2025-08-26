using JetBrains.Annotations;

namespace Kontecg.BlobStoring
{
    public interface IBlobProviderSelector
    {
        [NotNull]
        IBlobProvider Get([NotNull] string containerName);
    }
}