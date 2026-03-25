using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
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
    public class SendResetSuccessfulEmailHandler
      : INotificationHandler<PasswordResetTokenUsedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<SendResetSuccessfulEmailHandler> _logger;
        private readonly EmailOptions _options;
        private readonly IHostEnvironment _environment;

        public SendResetSuccessfulEmailHandler(
                IUserRepository userRepository,
                IResetPasswordRepository resetPasswordRepository,
                IEmailService emailService,
                ILogger<SendResetSuccessfulEmailHandler> logger,
                IOptions<EmailOptions> options,
                IHostEnvironment environment)
        {
            _userRepository = userRepository;
            _resetPasswordRepository = resetPasswordRepository;
            _emailService = emailService;
            _logger = logger;
            _options = options.Value;
            _environment = environment;
        }
        public async Task Handle(PasswordResetTokenUsedEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogError("Failed to send password reset success email.");
                return;
            }

            var activeTokens = await _resetPasswordRepository.GetAllActiveUserTokens(user.Id, cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            var body = $"""
            <p>Hello {user.FirstName},</p>

            <p>Your password for your <strong>Beddin</strong> account has been successfully reset.</p>

            <p>If you made this change, no further action is required.</p>

            <p>If you did not reset your password, please contact our support team immediately to secure your account.</p>

            <p>— Beddin Team</p>
            """;

            try
            {
                //await _emailService.SendAsync(
                //    to: notification.Email,
                //    subject: "Password Successfully Reset - Beddin",
                //    body: body,
                //    ct: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send password success email to {Email}. Password was reset successfully — resend manually if needed.",
                    user.Email);
            }
        }
    }
}
