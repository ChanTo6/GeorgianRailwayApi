using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using GeorgianRailwayApi.Services.Email;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace GeorgianRailwayApi.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                
                var smtpServer = _config["Email:SmtpServer"];
                var smtpPort = int.Parse(_config["Email:SmtpPort"]);
                var smtpUsername = _config["Email:SmtpUsername"];
                var smtpPassword = _config["Email:SmtpPassword"];
                var fromAddress = _config["Email:FromAddress"];

                if (string.IsNullOrEmpty(smtpPassword))
                {
                    throw new Exception("SMTP password is missing. Set it securely in appsettings.json or environment variables.");
                }

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(fromAddress));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(smtpUsername, smtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }
    }
}