namespace Kontecg.Configuration.Startup
{
    /// <summary>
    ///     Used to provide a way to configure modules.
    ///     Create entension methods to this class to be used over <see cref="IKontecgStartupConfiguration.Modules" /> object.
    /// </summary>
    public interface IModuleConfigurations
    {
        /// <summary>
        ///     Gets the Kontecg configuration object.
        /// </summary>
        IKontecgStartupConfiguration KontecgConfiguration { get; }
    }
}
