using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Beddin.Application.Features.Users.Commands.RefreshToken
{
    public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<RefreshTokenResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public RefreshTokenHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ApiResponse<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByRefreshToken(request.RefreshToken, cancellationToken);

            if (user == null)
            {
                return ApiResponse<RefreshTokenResponse>.Fail("Invalid refresh token.");
            }

            if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return ApiResponse<RefreshTokenResponse>.Fail("Invalid refresh token.");
            }

            if (!user.IsActive)
            {
                return ApiResponse<RefreshTokenResponse>.Fail("Invalid refresh token.");
            }


            var tokenHash = UserSession.ComputeHash(request.RefreshToken);
            var session = await _sessionRepository.GetByTokenHash(tokenHash, cancellationToken);

            if (session == null)
            {
                return ApiResponse<RefreshTokenResponse>.Fail("Invalid refresh token.");
            }

            if (!session.IsActive)
            {
                var allSessions = await _sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
                foreach (var s in allSessions)
                    s.Invalidate("Refresh token reuse detected");

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<RefreshTokenResponse>.Fail("Invalid refresh token.");
            }

            var activeSessions = await _sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var activeSession in activeSessions)
            {
                activeSession.Invalidate("Token refreshed");
            }

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var expirationMinutes = int.TryParse(_configuration["Jwt:RefreshTokenExpiryMinutes"], out var mins) ? mins : 10080;
            var newRefreshTokenExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);

            user.SetRefreshToken(newRefreshToken, newRefreshTokenExpiry);

            var newSession = UserSession.Create(
                user.Id,
                newRefreshToken,
                newRefreshTokenExpiry,
                request.IpAddress,
                request.UserAgent
            );

            await _sessionRepository.Add(newSession, cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user, newSession.Id.Value);


            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<RefreshTokenResponse>.Ok(new RefreshTokenResponse(
                accessToken,
                newRefreshToken
            ));
        }
    }
}
