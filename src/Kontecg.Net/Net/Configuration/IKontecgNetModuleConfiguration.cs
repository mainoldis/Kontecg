namespace Kontecg.Net.Configuration
{
    public interface IKontecgNetModuleConfiguration
    {
        bool RunAsServer { get; set; }

        bool UseSsl { get; set; }

        bool UseLibuv { get; set; }

        string ServerCertificate { get; set; }

        int Port { get; set; }
    }
}
