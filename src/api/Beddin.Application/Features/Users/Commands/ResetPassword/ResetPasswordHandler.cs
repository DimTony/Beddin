using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.RegisterUser;
using Beddin.Domain.Aggregates.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.ResetPassword
{
    public sealed class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RequestPasswordResetHandler(
            IUserRepository userRepository,
            IResetPasswordRepository resetPasswordRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _resetPasswordRepository = resetPasswordRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.IpAddress) || string.IsNullOrEmpty(request.UserAgent))
                return ApiResponse<bool>.Fail("IpAddress and UserAgent are required");

            if (string.IsNullOrEmpty(request.Email))
                return ApiResponse<bool>.Fail("An error occured resetting password");

            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
                return ApiResponse<bool>.Fail("An error occured resetting password");

            var resetToken = PasswordResetToken.Create(
                user.Id,
                request.IpAddress,
                request.UserAgent
                //ipAddress: _httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                //userAgent: _httpContext.Request.Headers.UserAgent.ToString()
            );

            await _resetPasswordRepository.AddAsync(resetToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Password reset, check your email");
        }
    }

    public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordHandler(
            IUserRepository userRepository,
            IResetPasswordRepository resetPasswordRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _resetPasswordRepository = resetPasswordRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            

            if (string.IsNullOrEmpty(request.Email))
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");

            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");

            var resetToken = await _resetPasswordRepository.GetByToken(request.Token, cancellationToken);

            if (resetToken is null || !resetToken.IsValid())
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");

            var tokenOwner = await _userRepository.GetByIdAsync(resetToken.UserId, cancellationToken);

            if (tokenOwner == null || user.Id != tokenOwner.Id)
                return ApiResponse<bool>.Fail("Token is invalid or has expired.");

            var newPasswordSameAsCurrent = BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash);

            if (newPasswordSameAsCurrent)
            {
                // Log failed attempt
                //user.AddDomainEvent(new UserPasswordChangeFailedEvent(user.Id, "New password cannot be the same as the current password"));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<bool>.Fail("New password cannot be the same as the current password.");
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var resetResult = tokenOwner.ResetPassword(newPasswordHash);

            if (!resetResult.IsSuccess)
                return ApiResponse<bool>.Fail($"Reset Failed. {resetResult.Error} ");

            resetToken.Use();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Password reset successfully!");

        }
        
    }
}
