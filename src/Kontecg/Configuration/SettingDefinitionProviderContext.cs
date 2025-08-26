namespace Kontecg.Configuration
{
    /// <summary>
    ///     The context that is used in setting providers.
    /// </summary>
    public class SettingDefinitionProviderContext
    {
        public SettingDefinitionProviderContext(ISettingDefinitionManager manager)
        {
            Manager = manager;
        }

        public ISettingDefinitionManager Manager { get; }
    }
}
