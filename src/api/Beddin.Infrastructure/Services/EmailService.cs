using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailOptions _options;
        public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(_options.FromEmail) || string.IsNullOrEmpty(_options.Password))
            {
                _logger.LogWarning("Email send skipped — FromEmail or Password not configured.");
                return;
            }

            try
            {
                using var client = new SmtpClient(_options.SmtpServer, _options.Port)
                {
                    Credentials = new NetworkCredential(_options.Username, _options.Password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_options.FromEmail, _options.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage, ct);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", to, subject);
                throw;
            }
        }

        public Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            _logger.LogInformation("Sending email confirmation to {Email}. Link: {Link}", email, confirmationLink);
            // TODO: Implement actual email sending (SendGrid, SMTP, etc.)
            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(string email, string resetLink)
        {
            _logger.LogInformation("Sending password reset to {Email}. Link: {Link}", email, resetLink);
            // TODO: Implement actual email sending
            return Task.CompletedTask;
        }

        public Task SendAccountLockedAsync(string email)
        {
            _logger.LogInformation("Sending account locked notification to {Email}", email);
            // TODO: Implement actual email sending
            return Task.CompletedTask;
        }

        public Task SendWelcomeEmailAsync(string email, string firstName)
        {
            _logger.LogInformation("Sending welcome email to {Email}", email);
            // TODO: Implement actual email sending
            return Task.CompletedTask;
        }
    }
}
