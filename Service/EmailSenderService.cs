using EmailSender.Interface;
using EmailSender.Model;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace EmailSender.Service
{

    public class EmailSenderService : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        public EmailSenderService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<string> SendEmailAsync(EmailRequest request)
        {
            var recipients = _smtpSettings.ReceiverEmailAddresses
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
            
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
            foreach (var recipientEmail in recipients)
            {
                message.To.Add(MailboxAddress.Parse(recipientEmail));
            }
            message.Subject = _smtpSettings.Subject + request.Name;
            message.Body = new TextPart("plain")
            {
                Text = $@"You have received a new message from your website!

Name: {request.Name}
Email: {request.Email}

Message:
{request.Message}"
            };

            var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, _smtpSettings.SSL);
                await client.AuthenticateAsync(new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password));
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return "Email Sent Successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
