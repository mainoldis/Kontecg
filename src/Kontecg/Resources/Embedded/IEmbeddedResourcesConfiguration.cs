using System.Collections.Generic;

namespace Kontecg.Resources.Embedded
{
    public interface IEmbeddedResourcesConfiguration
    {
        List<EmbeddedResourceSet> Sources { get; }
    }
}
