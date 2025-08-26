namespace Kontecg.DynamicEntityProperties
{
    public class DynamicEntityPropertyDefinitionContext : IDynamicEntityPropertyDefinitionContext
    {
        public DynamicEntityPropertyDefinitionContext()
        {
            Manager = NullDynamicEntityPropertyDefinitionManager.Instance;
        }

        public IDynamicEntityPropertyDefinitionManager Manager { get; set; }
    }
}
