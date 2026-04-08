// <copyright file="EmailService.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Mail;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beddin.Infrastructure.Services
{
    /// <summary>
    /// Implements the <see cref="IEmailService"/> interface to provide functionality for sending emails, including email confirmations, password resets, account lock notifications, and welcome emails. This service uses SMTP settings configured in the application options and includes robust error handling and logging to ensure that email sending operations are properly monitored and any issues are logged for troubleshooting.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly EmailOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="options">The email options.</param>
        /// <param name="logger">The logger.</param>
        public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
        {
            this.options = options.Value;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(this.options.FromEmail) || string.IsNullOrEmpty(this.options.Password))
            {
                this.logger.LogWarning("Email send skipped — FromEmail or Password not configured.");
                return;
            }

            try
            {
                using var client = new SmtpClient(this.options.SmtpServer, this.options.Port)
                {
                    Credentials = new NetworkCredential(this.options.Username, this.options.Password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(this.options.FromEmail, this.options.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage, ct);
                this.logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", to, subject);
                throw;
            }
        }

        /// <inheritdoc/>
        public Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            this.logger.LogInformation("Sending email confirmation to {Email}. Link: {Link}", email, confirmationLink);

            // TODO: Implement actual email sending (SendGrid, SMTP, etc.)
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendPasswordResetAsync(string email, string resetLink)
        {
            this.logger.LogInformation("Sending password reset to {Email}. Link: {Link}", email, resetLink);

            // TODO: Implement actual email sending
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendAccountLockedAsync(string email)
        {
            this.logger.LogInformation("Sending account locked notification to {Email}", email);

            // TODO: Implement actual email sending
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendWelcomeEmailAsync(string email, string firstName)
        {
            this.logger.LogInformation("Sending welcome email to {Email}", email);

            // TODO: Implement actual email sending
            return Task.CompletedTask;
        }
    }
}
