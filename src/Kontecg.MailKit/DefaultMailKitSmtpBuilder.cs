using Kontecg.Dependency;
using Kontecg.Net.Mail.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Kontecg.MailKit
{
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder, ITransientDependency
    {
        private readonly IKontecgMailKitConfiguration _kontecgMailKitConfiguration;
        private readonly ISmtpEmailSenderConfiguration _smtpEmailSenderConfiguration;

        public DefaultMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
            IKontecgMailKitConfiguration kontecgMailKitConfiguration)
        {
            _smtpEmailSenderConfiguration = smtpEmailSenderConfiguration;
            _kontecgMailKitConfiguration = kontecgMailKitConfiguration;
        }

        public virtual SmtpClient Build()
        {
            SmtpClient client = new SmtpClient();

            try
            {
                ConfigureClient(client);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        protected virtual void ConfigureClient(SmtpClient client)
        {
            client.Connect(
                _smtpEmailSenderConfiguration.Host,
                _smtpEmailSenderConfiguration.Port,
                GetSecureSocketOption()
            );

            if (_smtpEmailSenderConfiguration.UseDefaultCredentials)
            {
                return;
            }

            client.Authenticate(
                _smtpEmailSenderConfiguration.UserName,
                _smtpEmailSenderConfiguration.Password
            );
        }

        protected virtual SecureSocketOptions GetSecureSocketOption()
        {
            if (_kontecgMailKitConfiguration.SecureSocketOption.HasValue)
            {
                return _kontecgMailKitConfiguration.SecureSocketOption.Value;
            }

            return _smtpEmailSenderConfiguration.EnableSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;
        }
    }
}
