// <copyright file="SendRequestEmail.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Net;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Beddin.Domain.Events;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beddin.Application.Notifications.OnPasswordReset
{
#pragma warning disable SA1649 // File name should match first type name
    /// <summary>
    /// Handles the sending of password reset emails when a <see cref="PasswordResetTokenCreatedEvent"/> is raised.
    /// </summary>
    public class SendPasswordResetEmailHandler
#pragma warning restore SA1649 // File name should match first type name
       : INotificationHandler<PasswordResetTokenCreatedEvent>
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<SendPasswordResetEmailHandler> logger;
        private readonly EmailOptions options;
        private readonly IHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPasswordResetEmailHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The e-mail configuration options.</param>
        /// <param name="environment">The hosting environment.</param>
        public SendPasswordResetEmailHandler(
                IUserRepository userRepository,
                ILogger<SendPasswordResetEmailHandler> logger,
                IOptions<EmailOptions> options,
                IHostEnvironment environment)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.options = options.Value;
            this.environment = environment;
        }

        /// <inheritdoc/>
        public async Task Handle(PasswordResetTokenCreatedEvent notification, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetByIdAsync(notification.UserId, cancellationToken);

            if (user == null)
            {
                this.logger.LogError("Failed to send password reset email.");
                return;
            }

            var encodedToken = WebUtility.UrlEncode(notification.Token);

            var confirmationLink = $"{this.options.BaseUrl}/auth/password-reset?email={user.Email}&token={encodedToken}";

            if (this.environment.IsDevelopment())
            {
                this.logger.LogInformation("DEV PASSWORD RESET LINK: {Link}", confirmationLink);

                // Optional: also write to console directly
                Console.WriteLine($"DEV PASSWORD RESET LINK: {confirmationLink}");
            }

            var body = $"""
            <p>Hello {user.FirstName},</p>

            <p>We received a request to reset your password for your <strong>Beddin</strong> account.</p>

            <p>You can reset your password by clicking the link below:</p>

            <p>
                <a href="{confirmationLink}">Reset Your Password</a>
            </p>

            <p>This link will expire after a limited time for security reasons.</p>

            <p>If you did not request a password reset, please ignore this email. Your account will remain secure.</p>

            <p>— Beddin Team</p>
            """;

            try
            {
                // await _emailService.SendAsync(
                //    to: notification.Email,
                //    subject: "Confirm your email - Beddin",
                //    body: body,
                //    ct: cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Failed to send onboarding email to {Email}. User account was created successfully — resend manually if needed.",
                    user.Email);
            }
        }
    }
}
