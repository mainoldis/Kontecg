using MailKit.Net.Smtp;

namespace Kontecg.MailKit
{
    public interface IMailKitSmtpBuilder
    {
        SmtpClient Build();
    }
}
