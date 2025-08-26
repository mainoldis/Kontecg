using MailKit.Security;

namespace Kontecg.MailKit
{
    public interface IKontecgMailKitConfiguration
    {
        SecureSocketOptions? SecureSocketOption { get; set; }
    }
}
