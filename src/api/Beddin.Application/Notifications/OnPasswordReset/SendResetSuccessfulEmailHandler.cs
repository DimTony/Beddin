// <copyright file="SendResetSuccessfulEmailHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Beddin.Domain.Events;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beddin.Application.Notifications.OnPasswordReset
{
    /// <summary>
    /// Handles the event when a password reset token is used by revoking all active tokens and sending a notification email.
    /// </summary>
    public class SendResetSuccessfulEmailHandler
      : INotificationHandler<PasswordResetTokenUsedEvent>
    {
        private readonly IUserRepository userRepository;
        private readonly IResetPasswordRepository resetPasswordRepository;
        private readonly ILogger<SendResetSuccessfulEmailHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendResetSuccessfulEmailHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="resetPasswordRepository">The reset password repository.</param>
        /// <param name="logger">The logger instance.</param>
        public SendResetSuccessfulEmailHandler(
                IUserRepository userRepository,
                IResetPasswordRepository resetPasswordRepository,
                ILogger<SendResetSuccessfulEmailHandler> logger)
        {
            this.userRepository = userRepository;
            this.resetPasswordRepository = resetPasswordRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Handles the <see cref="PasswordResetTokenUsedEvent"/> by revoking all active tokens and sending a notification email.
        /// </summary>
        /// <param name="notification">The event notification.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Handle(PasswordResetTokenUsedEvent notification, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetByIdAsync(notification.UserId, cancellationToken);
            if (user == null)
            {
                this.logger.LogError("Failed to send password reset success email.");
                return;
            }

            var activeTokens = await this.resetPasswordRepository.GetAllActiveUserTokens(user.Id, cancellationToken);
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
                // await _emailService.SendAsync(to: notification.Email, subject: "Password Successfully Reset - Beddin", body: body, ct: cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Failed to send password success email to {Email}. Password was reset successfully — resend manually if needed.",
                    user.Email);
            }
        }
    }
}
