namespace Kontecg.Net.Configuration
{
    internal class KontecgNetModuleConfiguration : IKontecgNetModuleConfiguration
    {
        public KontecgNetModuleConfiguration()
        {
            RunAsServer = true;
            Port = 8001;
            UseSsl = false;
            UseLibuv = true;
        }

        public bool RunAsServer { get; set; }
        public bool UseSsl { get; set; }
        public bool UseLibuv { get; set; }
        public string ServerCertificate { get; set; }
        public int Port { get; set; }
    }
}
