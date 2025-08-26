using System.Collections.Generic;

namespace Kontecg.Resources.Embedded
{
    public class EmbeddedResourcesConfiguration : IEmbeddedResourcesConfiguration
    {
        public EmbeddedResourcesConfiguration()
        {
            Sources = new List<EmbeddedResourceSet>();
        }

        public List<EmbeddedResourceSet> Sources { get; }
    }
}
