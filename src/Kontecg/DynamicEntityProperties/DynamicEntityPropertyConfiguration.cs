using Kontecg.Collections;

namespace Kontecg.DynamicEntityProperties
{
    public class DynamicEntityPropertyConfiguration : IDynamicEntityPropertyConfiguration
    {
        public DynamicEntityPropertyConfiguration()
        {
            Providers = new TypeList<DynamicEntityPropertyDefinitionProvider>();
        }

        public ITypeList<DynamicEntityPropertyDefinitionProvider> Providers { get; }
    }
}
