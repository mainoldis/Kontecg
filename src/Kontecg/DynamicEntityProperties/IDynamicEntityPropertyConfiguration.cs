using Kontecg.Collections;

namespace Kontecg.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyConfiguration
    {
        ITypeList<DynamicEntityPropertyDefinitionProvider> Providers { get; }
    }
}
