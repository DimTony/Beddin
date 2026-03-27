using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.Login;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, ApiResponse<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;


        public ConfirmEmailHandler(IUserRepository userRepository, IUserSessionRepository sessionRepository, IConfiguration configuration, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<LoginResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null || user.EmailConfirmed)
                return ApiResponse<LoginResponse>.Fail("Invalid email or token.");

            var decodedToken = WebUtility.UrlDecode(request.Token);

            var activeSessions = await _sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var activeSession in activeSessions)
            {
                activeSession.Invalidate();
            }

            var refreshToken = _tokenService.GenerateRefreshToken();
            var expirationMinutes = int.TryParse(_configuration["Jwt:RefreshTokenExpiryMinutes"], out var mins) ? mins : 10080;
            var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var result = user.ConfirmEmailAndActivate(decodedToken, refreshToken, refreshTokenExpiry, DateTime.UtcNow);
            if (!result.IsSuccess)
            {
                return ApiResponse<LoginResponse>.Fail(result.Error);
            }

            var session = UserSession.Create(
                user.Id,
                refreshToken,
                refreshTokenExpiry,
                request.IpAddress,
                request.UserAgent
            );

            await _sessionRepository.Add(session, cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user, session.Id.Value);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<LoginResponse>.Ok(new LoginResponse(
                accessToken,
                refreshToken
            ), "Email confirmed successfully!");

        }
    }
}
