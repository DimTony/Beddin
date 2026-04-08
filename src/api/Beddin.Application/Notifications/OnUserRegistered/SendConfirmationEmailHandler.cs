// <copyright file="SendConfirmationEmailHandler.cs" company="Beddin">
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

namespace Beddin.Application.Notifications.OnUserRegistered
{
    /// <summary>
    /// Handles the <see cref="EmailConfirmationTokenGeneratedEvent"/> by sending a confirmation email to the user.
    /// </summary>
    public class SendConfirmationEmailHandler
       : INotificationHandler<EmailConfirmationTokenGeneratedEvent>
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;
        private readonly ILogger<SendConfirmationEmailHandler> logger;
        private readonly EmailOptions options;
        private readonly IHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendConfirmationEmailHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The email options.</param>
        /// <param name="environment">The host environment.</param>
        public SendConfirmationEmailHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<SendConfirmationEmailHandler> logger,
            IOptions<EmailOptions> options,
            IHostEnvironment environment)
        {
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.logger = logger;
            this.options = options.Value;
            this.environment = environment;
        }

        /// <summary>
        /// Handles the event when an email confirmation token is generated for a user.
        /// Sends a confirmation email to the user.
        /// </summary>
        /// <param name="notification">The event notification containing user and token information.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle(EmailConfirmationTokenGeneratedEvent notification, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetByIdAsync(notification.UserId, cancellationToken);

            if (user == null)
            {
                this.logger.LogError("User with email {Email} not found for sending confirmation email.", notification.Email);
                return;
            }

            var encodedToken = WebUtility.UrlEncode(notification.Token);

            var confirmationLink = $"{this.options.BaseUrl}/auth/confirm-email?email={notification.Email}&token={encodedToken}";

            if (this.environment.IsDevelopment())
            {
                this.logger.LogInformation("DEV EMAIL CONFIRMATION LINK: {Link}", confirmationLink);

                // Optional: also write to console directly
                Console.WriteLine($"DEV EMAIL CONFIRMATION LINK: {confirmationLink}");
            }

            var body = $"""
            <p>Hello {notification.FirstName},</p>

            <p>Your account has been successfully created on <strong>Beddin</strong>.</p>

            <p>Please confirm your email address by clicking the link below:</p>

            <p>
                <a href="{confirmationLink}">Confirm Your Email</a>
            </p>

            <p>This link will expire after a limited time for security reasons.</p>

            <p>If you did not create this account, please ignore this email.</p>

            <p>— Beddin Team</p>
            """;

            try
            {
                await this.emailService.SendAsync(
                    to: notification.Email,
                    subject: "Confirm your email - Beddin",
                    body: body,
                    ct: cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Failed to send onboarding email to {Email}. User account was created successfully — resend manually if needed.",
                    notification.Email);
            }
        }
    }
}
