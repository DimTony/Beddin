// <copyright file="ChangePasswordHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.ChangePassword
{
    /// <summary>
    /// Handles the change password command for a user.
    /// </summary>
    public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<string>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserSessionRepository sessionRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="sessionRepository">The user session repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="currentUser">The current user service.</param>
        public ChangePasswordHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            this.userRepository = userRepository;
            this.sessionRepository = sessionRepository;
            this.unitOfWork = unitOfWork;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Handles the change password command.
        /// </summary>
        /// <param name="request">The change password command request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating the result of the operation.</returns>
        public async Task<ApiResponse<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return ApiResponse<string>.Fail("Passwords do not match.");
            }

            var userId = this.currentUser.UserId;

            if (!userId.HasValue)
            {
                return ApiResponse<string>.Fail("User not found.");
            }

            var user = await this.userRepository.GetByIdAsync(new UserId(userId.Value), cancellationToken);

            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found.");
            }

            var isValidCurrentPassword = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);

            if (!isValidCurrentPassword)
            {
                // Log failed attempt
                await this.unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<string>.Fail("Current password is incorrect.");
            }

            var newPasswordSameAsCurrent = BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash);

            if (newPasswordSameAsCurrent)
            {
                // Log failed attempt
                await this.unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<string>.Fail("New password cannot be the same as the current password.");
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var result = user.ChangePassword(newPasswordHash);

            if (!result.IsSuccess)
            {
                return ApiResponse<string>.Fail(result.Error!);
            }

            // Invalidate all sessions — force re-login everywhere
            var sessions = await this.sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var session in sessions)
            {
                session.Invalidate("Password changed");
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.Ok("Password changed successfully.");
        }
    }
}
