using Beddin.Application.Common.DTOs;
//using Beddin.Application;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Application.Features.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Beddin.Application.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public ChangePasswordHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return ApiResponse<string>.Fail("Passwords do not match.");

            var userId = _currentUser.UserId;

            if (!userId.HasValue)
                return ApiResponse<string>.Fail("User not found.");

            var user = await _userRepository.GetByIdAsync(new UserId(userId.Value), cancellationToken);

            if (user == null)
                return ApiResponse<string>.Fail("User not found.");

            var isValidCurrentPassword = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);

            if (!isValidCurrentPassword)
            {
                // Log failed attempt
                //user.AddDomainEvent(new UserPasswordChangeFailedEvent(user.Id, "Invalid current password"));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<string>.Fail("Current password is incorrect.");
            }

            var newPasswordSameAsCurrent = BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash);

            if (newPasswordSameAsCurrent)
            {
                // Log failed attempt
                //user.AddDomainEvent(new UserPasswordChangeFailedEvent(user.Id, "New password cannot be the same as the current password"));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<string>.Fail("New password cannot be the same as the current password.");
            }   

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var result = user.ChangePassword(newPasswordHash);

            if (!result.IsSuccess)
                return ApiResponse<string>.Fail(result.Error!);

            // Invalidate all sessions — force re-login everywhere
            var sessions = await _sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var session in sessions)
                session.Invalidate("Password changed");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.Ok("Password changed successfully.");
        }
    }
}
