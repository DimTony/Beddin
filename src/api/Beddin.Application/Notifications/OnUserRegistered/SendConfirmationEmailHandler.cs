using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Notifications.OnUserRegistered
{
    public class SendConfirmationEmailHandler
       : INotificationHandler<EmailConfirmationTokenGeneratedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<SendConfirmationEmailHandler> _logger;
        private readonly EmailOptions _options;
        private readonly IHostEnvironment _environment;

        public SendConfirmationEmailHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<SendConfirmationEmailHandler> logger,
            IOptions<EmailOptions> options,
            IHostEnvironment environment)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
            _options = options.Value;
            _environment = environment;
        }
        public async Task Handle(EmailConfirmationTokenGeneratedEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogError("User with email {Email} not found for sending confirmation email.", notification.Email);
                return;
            }

            var encodedToken = WebUtility.UrlEncode(notification.Token);

            var confirmationLink = $"{_options.BaseUrl}/auth/confirm-email?email={notification.Email}&token={encodedToken}";

            if (_environment.IsDevelopment())
            {
                _logger.LogInformation("DEV EMAIL CONFIRMATION LINK: {Link}", confirmationLink);

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
                await _emailService.SendAsync(
                    to: notification.Email,
                    subject: "Confirm your email - Beddin",
                    body: body,
                    ct: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send onboarding email to {Email}. User account was created successfully — resend manually if needed.",
                    notification.Email);
            }
        }
    }
}
