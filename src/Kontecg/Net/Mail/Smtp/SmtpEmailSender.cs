using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Extensions;

namespace Kontecg.Net.Mail.Smtp
{
    /// <summary>
    ///     Used to send emails over SMTP.
    /// </summary>
    public class SmtpEmailSender : EmailSenderBase, ISmtpEmailSender, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration _configuration;

        /// <summary>
        ///     Creates a new <see cref="SmtpEmailSender" />.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public SmtpEmailSender(ISmtpEmailSenderConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public SmtpClient BuildClient()
        {
            string host = _configuration.Host;
            int port = _configuration.Port;

            SmtpClient smtpClient = new SmtpClient(host, port);
            try
            {
                if (_configuration.EnableSsl)
                {
                    smtpClient.EnableSsl = true;
                }

                if (_configuration.UseDefaultCredentials)
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = false;

                    string userName = _configuration.UserName;
                    if (!userName.IsNullOrEmpty())
                    {
                        string password = _configuration.Password;
                        string domain = _configuration.Domain;
                        smtpClient.Credentials = !domain.IsNullOrEmpty()
                            ? new NetworkCredential(userName, password, domain)
                            : new NetworkCredential(userName, password);
                    }
                }

                return smtpClient;
            }
            catch
            {
                smtpClient.Dispose();
                throw;
            }
        }

        protected override async Task SendEmailAsync(MailMessage mail)
        {
            using SmtpClient smtpClient = BuildClient();
            await smtpClient.SendMailAsync(mail);
        }

        protected override void SendEmail(MailMessage mail)
        {
            using SmtpClient smtpClient = BuildClient();
            smtpClient.Send(mail);
        }
    }
}
