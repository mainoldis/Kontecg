using System.Security.Cryptography.X509Certificates;

namespace Kontecg.Net.Security
{
    public interface ICertificateProvider
    {
        X509Certificate GetSelfSignedCertificate();
    }
}
