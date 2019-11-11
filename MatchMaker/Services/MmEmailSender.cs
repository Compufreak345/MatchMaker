using MailKit.Net.Smtp;
using MatchMaker.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Services
{
    public class MmEmailSender : IEmailSender
    {
        private readonly Email settings;

        public MmEmailSender(IOptions<Email> options)
        {
            this.settings = options.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(this.settings.SenderName, this.settings.SenderAddress));
            message.To.Add(new MailboxAddress("Du Stueck", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(this.settings.Server, this.settings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                
                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(this.settings.UserName, this.settings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
