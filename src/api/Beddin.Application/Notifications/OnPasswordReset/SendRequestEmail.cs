using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Beddin.Application.Notifications.OnUserRegistered;
using Beddin.Domain.Events;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Notifications.OnPasswordReset
{
    public class SendPasswordResetEmailHandler
       : INotificationHandler<PasswordResetTokenCreatedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<SendPasswordResetEmailHandler> _logger;
        private readonly EmailOptions _options;
        private readonly IHostEnvironment _environment;

        public SendPasswordResetEmailHandler(
                IUserRepository userRepository,
                IEmailService emailService,
                ILogger<SendPasswordResetEmailHandler> logger,
                IOptions<EmailOptions> options,
                IHostEnvironment environment)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
            _options = options.Value;
            _environment = environment;
        }
        public async Task Handle(PasswordResetTokenCreatedEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogError("Failed to send password reset email.");
                return;
            }

            var encodedToken = WebUtility.UrlEncode(notification.Token);

            var confirmationLink = $"{_options.BaseUrl}/auth/password-reset?email={user.Email}&token={encodedToken}";

            if (_environment.IsDevelopment())
            {
                _logger.LogInformation("DEV PASSWORD RESET LINK: {Link}", confirmationLink);

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
                //await _emailService.SendAsync(
                //    to: notification.Email,
                //    subject: "Confirm your email - Beddin",
                //    body: body,
                //    ct: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send onboarding email to {Email}. User account was created successfully — resend manually if needed.",
                    user.Email);
            }
        }
    }
}
