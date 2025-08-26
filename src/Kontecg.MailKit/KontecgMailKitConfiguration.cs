using MailKit.Security;

namespace Kontecg.MailKit
{
    public class KontecgMailKitConfiguration : IKontecgMailKitConfiguration
    {
        public SecureSocketOptions? SecureSocketOption { get; set; }
    }
}
