namespace Kontecg.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public ModuleConfigurations(IKontecgStartupConfiguration kontecgConfiguration)
        {
            KontecgConfiguration = kontecgConfiguration;
        }

        public IKontecgStartupConfiguration KontecgConfiguration { get; }
    }
}
