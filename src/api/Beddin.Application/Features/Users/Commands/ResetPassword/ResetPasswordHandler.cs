// <copyright file="ResetPasswordHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.ResetPassword
{
    /// <summary>
    /// Handler for the <see cref="RequestPasswordResetCommand"/>.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
    public sealed class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, ApiResponse<bool>>
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
    {
        private readonly IUserRepository userRepository;
        private readonly IResetPasswordRepository resetPasswordRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPasswordResetHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="resetPasswordRepository">The reset password repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public RequestPasswordResetHandler(
            IUserRepository userRepository,
            IResetPasswordRepository resetPasswordRepository,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.resetPasswordRepository = resetPasswordRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.IpAddress) || string.IsNullOrEmpty(request.UserAgent))
            {
                return ApiResponse<bool>.Fail("IpAddress and UserAgent are required");
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                return ApiResponse<bool>.Fail("An error occured resetting password");
            }

            var user = await this.userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("An error occured resetting password");
            }

            var resetToken = PasswordResetToken.Create(
                user.Id,
                request.IpAddress,
                request.UserAgent);

            await this.resetPasswordRepository.AddAsync(resetToken, cancellationToken);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Password reset, check your email");
        }
    }

    /// <summary>
    /// Handler for the <see cref="ResetPasswordCommand"/>.
    /// </summary>
    public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IResetPasswordRepository resetPasswordRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="resetPasswordRepository">The reset password repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public ResetPasswordHandler(
            IUserRepository userRepository,
            IResetPasswordRepository resetPasswordRepository,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.resetPasswordRepository = resetPasswordRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");
            }

            var user = await this.userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");
            }

            var resetToken = await this.resetPasswordRepository.GetByToken(request.Token, cancellationToken);

            if (resetToken is null || !resetToken.IsValid())
            {
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");
            }

            var tokenOwner = await this.userRepository.GetByIdAsync(resetToken.UserId, cancellationToken);

            if (tokenOwner == null || user.Id != tokenOwner.Id)
            {
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");
            }

            var newPasswordSameAsCurrent = BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash);

            if (newPasswordSameAsCurrent)
            {
                await this.unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<bool>.Fail("New password cannot be the same as the current password.");
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var resetResult = tokenOwner.ResetPassword(newPasswordHash);

            if (!resetResult.IsSuccess)
            {
                return ApiResponse<bool>.Fail($"Reset Failed. {resetResult.Error} ");
            }

            resetToken.Use();

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Password reset successfully!");
        }
    }
}
