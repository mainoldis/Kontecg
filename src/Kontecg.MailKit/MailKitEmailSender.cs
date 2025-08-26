using System.Net.Mail;
using System.Threading.Tasks;
using Kontecg.Net.Mail;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Kontecg.MailKit
{
    public class MailKitEmailSender : EmailSenderBase
    {
        private readonly IMailKitSmtpBuilder _smtpBuilder;

        public MailKitEmailSender(
            IEmailSenderConfiguration smtpEmailSenderConfiguration,
            IMailKitSmtpBuilder smtpBuilder)
            : base(
                smtpEmailSenderConfiguration)
        {
            _smtpBuilder = smtpBuilder;
        }

        public override async Task SendAsync(string from, string to, string subject, string body,
            bool isBodyHtml = true)
        {
            using SmtpClient client = BuildSmtpClient();
            MimeMessage message = BuildMimeMessage(from, to, subject, body, isBodyHtml);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public override void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            using SmtpClient client = BuildSmtpClient();
            MimeMessage message = BuildMimeMessage(from, to, subject, body, isBodyHtml);
            client.Send(message);
            client.Disconnect(true);
        }

        protected override async Task SendEmailAsync(MailMessage mail)
        {
            using SmtpClient client = BuildSmtpClient();
            MimeMessage message = MimeMessage.CreateFromMailMessage(mail);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        protected override void SendEmail(MailMessage mail)
        {
            using SmtpClient client = BuildSmtpClient();
            MimeMessage message = MimeMessage.CreateFromMailMessage(mail);
            client.Send(message);
            client.Disconnect(true);
        }

        protected virtual SmtpClient BuildSmtpClient()
        {
            return _smtpBuilder.Build();
        }

        private static MimeMessage BuildMimeMessage(string from, string to, string subject, string body,
            bool isBodyHtml = true)
        {
            string bodyType = isBodyHtml ? "html" : "plain";
            MimeMessage message = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart(bodyType)
                {
                    Text = body
                }
            };

            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(to));

            return message;
        }
    }
}
