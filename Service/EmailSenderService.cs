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

    // Build admin email body
    var adminBody = $@"
NOVA REGISTRACIJA TIMA - SARAJEVO LJUBAVI MOJA 2026

NAZIV TIMA: {request.TeamName}
DRŽAVA PORIJEKLA: {request.Country}
PREDSTAVNIK TIMA: {request.Representative}
KONTAKT TELEFON: {request.Phone}
KONTAKT EMAIL: {request.Email}

SPISAK TAKMIČARA:
{request.Players}

PLANIRANI DOLAZAK: {request.ArrivalDate}
PLANIRANI ODLAZAK: {request.DepartureDate}

VAŽNE NAPOMENE:
{request.Notes}
";

    var messageToAdmins = new MimeMessage();
    messageToAdmins.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
    foreach (var email in recipients)
    {
        messageToAdmins.To.Add(MailboxAddress.Parse(email));
    }

    messageToAdmins.Subject = $"Nova registracija tima - {request.TeamName}";
    messageToAdmins.Body = new TextPart("plain") { Text = adminBody };

    // Confirmation email to user
    var confirmationBody = $@"
Poštovani {request.Representative},

Vaša prijava za turnir 'Sarajevo Ljubavi Moja 2026' je uspješno zaprimljena.

Uskoro ćemo Vas kontaktirati sa dodatnim informacijama.

Lijep pozdrav,
Organizacioni tim RKV Sarajevo
";

            var messageToUser = new MimeMessage();
            messageToUser.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
            messageToUser.To.Add(MailboxAddress.Parse(request.Email));
            messageToUser.Subject = "Potvrda prijave - Sarajevo Ljubavi Moja 2026";
            messageToUser.Body = new TextPart("plain") { Text = confirmationBody };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, _smtpSettings.SSL);
                await client.AuthenticateAsync(_smtpSettings.SenderEmail, _smtpSettings.Password);

                // Send admin email
                await client.SendAsync(messageToAdmins);

                // Send confirmation to user
                await client.SendAsync(messageToUser);

                await client.DisconnectAsync(true);
                return "OK";
            }
            catch (Exception ex)
            {
                throw;
            }
}
        
 public async Task<string> SendContactEmailAsync(ContactEmailRequest request)
{
    var recipients = _smtpSettings.ReceiverEmailAddresses
        .Split(',', StringSplitOptions.RemoveEmptyEntries);

    // -------------------------------
    // EMAIL TO ADMINS
    // -------------------------------
    var adminBody = $@"
NOVA PORUKA SA KONTAKT FORME

Ime: {request.Name}
Email: {request.Email}

Poruka:
{request.Message}
";

    var adminMessage = new MimeMessage();
    adminMessage.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
    foreach (var email in recipients)
    {
        adminMessage.To.Add(MailboxAddress.Parse(email));
    }
    adminMessage.Subject = $"Kontakt forma: {request.Subject}";
    adminMessage.Body = new TextPart("plain") { Text = adminBody };

    // -------------------------------
    // CONFIRMATION EMAIL TO USER
    // -------------------------------
    var confirmationBody = $@"
Poštovani {request.Name},

Vaša poruka je uspješno zaprimljena.
Odgovorićemo Vam u najkraćem mogućem roku.

Lijep pozdrav,
RKV Sarajevo
";

    var userMessage = new MimeMessage();
    userMessage.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
    userMessage.To.Add(MailboxAddress.Parse(request.Email));
    userMessage.Subject = "Potvrda - Vaša poruka je zaprimljena";
    userMessage.Body = new TextPart("plain") { Text = confirmationBody };

    // -------------------------------
    // SEND EMAILS
    // -------------------------------
    using var client = new SmtpClient();

    try
    {
        await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, _smtpSettings.SSL);
        await client.AuthenticateAsync(_smtpSettings.SenderEmail, _smtpSettings.Password);

        // Send to admins
        await client.SendAsync(adminMessage);

        // Send confirmation to user
        await client.SendAsync(userMessage);

        await client.DisconnectAsync(true);
        return "OK";
    }
    catch (Exception)
    {
        throw;
    }
}

    }
}
