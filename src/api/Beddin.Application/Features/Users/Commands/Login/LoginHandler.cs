using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Beddin.Application.Features.Users.Commands.Login
{
    public sealed class LoginHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
    {
        private const int MaxFailedAttempts = 3;
        private const int LockoutMinutes = 30;

        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LoginHandler> _logger;
        private readonly IConfiguration _configuration;

        public LoginHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            ITokenService tokenService,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ILogger<LoginHandler> logger,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find identity user
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);
            if (user == null)
            {
                return ApiResponse<LoginResponse>.Fail("Invalid email or password.");
            }

            var activeSessions = await _sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var activeSession in activeSessions)
            {
                activeSession.Invalidate(); 
            }

            var refreshToken = _tokenService.GenerateRefreshToken();
            var expirationMinutes = int.TryParse(_configuration["Jwt:RefreshTokenExpiryMinutes"], out var mins) ? mins : 10080;
            var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var result = user.AttemptLogin(request.Password, refreshToken, refreshTokenExpiry, DateTime.UtcNow);

            if (!result.IsSuccess)
            {
                return ApiResponse<LoginResponse>.Fail(result.Error!);
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
            var expiresAt = _tokenService.GetTokenExpiration(accessToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<LoginResponse>.Ok(new LoginResponse(
                accessToken,
                refreshToken
            ), "Login successful!");
        }
    }

}
