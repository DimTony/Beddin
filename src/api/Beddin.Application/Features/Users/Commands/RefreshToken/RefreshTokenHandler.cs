using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.RefreshToken
{
    public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenHandler(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Find user by refresh token
            var identityUser = _userManager.Users
                .FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

            if (identityUser == null)
            {
                return Result.Failure<RefreshTokenResponse>("Invalid refresh token.");
            }

            // Check if refresh token is expired in Identity
            if (identityUser.RefreshTokenExpiry == null || identityUser.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return Result.Failure<RefreshTokenResponse>("Refresh token expired.");
            }

            // Check if account is active
            if (!identityUser.IsActive)
            {
                return Result.Failure<RefreshTokenResponse>("Account is deactivated.");
            }

            var userId = new UserId(Guid.Parse(identityUser.Id));

            // Validate session exists and is active
            var tokenHash = UserSession.ComputeHash(request.RefreshToken);
            var session = await _sessionRepository.GetByTokenHash(tokenHash, cancellationToken);

            if (session == null)
            {
                return Result.Failure<RefreshTokenResponse>("Session not found.");
            }

            if (!session.IsActive)
            {
                return Result.Failure<RefreshTokenResponse>("Session has been invalidated. Please log in again.");
            }

            // Get domain user
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return Result.Failure<RefreshTokenResponse>("User data not found.");
            }

            // Generate new tokens
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // Create new session (invalidate old one and create new)
            var invalidationResult = session.Invalidate("Token refreshed");
            if (invalidationResult.IsFailure)
            {
                return Result.Failure<RefreshTokenResponse>(invalidationResult.Error);
            }
            await _sessionRepository.Update(session, cancellationToken);

            // Create new session with new refresh token
            var newSession = UserSession.Create(
                userId,
                newRefreshToken,
                newRefreshTokenExpiry,
                request.IpAddress,
                request.UserAgent
            );

            await _sessionRepository.Add(newSession, cancellationToken);

            // Generate new access token with new session ID
            var accessToken = _tokenService.GenerateAccessToken(user, newSession.Id.Value);
            var expiresAt = _tokenService.GetTokenExpiration(accessToken);

            // Update refresh token in Identity user
            identityUser.RefreshToken = newRefreshToken;
            identityUser.RefreshTokenExpiry = newRefreshTokenExpiry;
            await _userManager.UpdateAsync(identityUser);

            // Commit all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new RefreshTokenResponse(
                accessToken,
                newRefreshToken,
                expiresAt,
                newSession.Id.Value
            ));
        }
    }
}
