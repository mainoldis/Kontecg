namespace Kontecg.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyDefinitionContext
    {
        /// <summary>
        ///     Gets the DynamicEntityProperty definition manager.
        /// </summary>
        IDynamicEntityPropertyDefinitionManager Manager { get; set; }
    }
}
