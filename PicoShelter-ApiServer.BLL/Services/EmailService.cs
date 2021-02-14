using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;
using PicoShelter_ApiServer.BLL.Formatters;
using PicoShelter_ApiServer.BLL.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class EmailService : IEmailService
    {
        private EmailAuthDto authDto { get; set; }
        private ILogger _logger { get; set; }
        public EmailService(EmailAuthDto emailAuth, ILogger<IEmailService> logger)
        {
            authDto = emailAuth;
            _logger = logger;
        }

        private async Task SendEmailAsync(string email, string subject, string messageHtml)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(authDto.from);
            emailMessage.To.Add(new MailboxAddress(string.Empty, email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = messageHtml
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(authDto.host, authDto.port, authDto.useSsl);
                    await client.AuthenticateAsync(authDto.username, authDto.password);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SmtpClient Connection/Authorization Exception");
                    throw;
                }
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }


        public async Task SendConfirmEmailAsync(EmailConfirmationDto dto)
        {
            EmailFormatter<EmailConfirmationDto> formatter = new EmailConfirmationFormatter();
            var msg = formatter.Format(dto);
            await SendEmailAsync(dto.targetEmail, "Email confirmation", msg);
        }

        public async Task SendPasswordRestoreEmailAsync(PasswordResetDto dto)
        {
            EmailFormatter<PasswordResetDto> formatter = new PasswordResetFormatter();
            var msg = formatter.Format(dto);
            await SendEmailAsync(dto.targetEmail, "Password reset", msg);
        }

        public async Task SendEmailChangingEmailAsync(EmailChangingDto dto)
        {
            EmailFormatter<EmailChangingDto> formatter = new EmailChangingFormatter();
            var msg = formatter.Format(dto);
            await SendEmailAsync(dto.targetEmail, "Email changing. Step 1/2", msg);
        }

        public async Task SendEmailChangingNewEmailAsync(EmailChangingNewDto dto)
        {
            EmailFormatter<EmailChangingNewDto> formatter = new EmailChangingNewFormatter();
            var msg = formatter.Format(dto);
            await SendEmailAsync(dto.targetEmail, "Email changing. Step 2/2", msg);
        }
    }
}
