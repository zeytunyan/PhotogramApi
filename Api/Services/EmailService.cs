using Api.Configs;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Common.MailMessages;
using static System.Net.WebRequestMethods;
using Api.Controllers;

namespace Api.Services
{
    public class EmailService
    {
        private readonly EmailConfig _config;
        private readonly ILogger _logger;

        public EmailService(IOptions<EmailConfig> config, ILogger<AuthController> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string adress, string adressee)
        {
            var fromAdress = new MailAddress(_config.UserName, _config.DisplayName);
            var toAdress = new MailAddress(adress, adressee);
            using var welcomeMail = new WelcomeMailMessage(fromAdress, toAdress);
            await SendEmailAsync(welcomeMail);
        }

        public async Task SendEmailAsync(MailMessage mail)
        {
            using var smtp = CreateSmtpClient();
            
            try
            {
                await smtp.SendMailAsync(mail); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private SmtpClient CreateSmtpClient() => new()
        {
            Host = _config.Host,
            Port = _config.Port,
            //EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_config.UserName, _config.Password)
        };
    }
}
