// <copyright file="LoginHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Beddin.Application.Features.Users.Commands.Login
{
    /// <summary>
    /// Handler for the <see cref="LoginCommand"/>.
    /// </summary>
    public sealed class LoginHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
    {
        private const int MaxFailedAttempts = 3;
        private const int LockoutMinutes = 30;

        private readonly IUserRepository userRepository;
        private readonly IUserSessionRepository sessionRepository;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LoginHandler> logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="sessionRepository">The session repository.</param>
        /// <param name="tokenService">The token service.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public LoginHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            ILogger<LoginHandler> logger,
            IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.sessionRepository = sessionRepository;
            this.tokenService = tokenService;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find identity user
            var user = await this.userRepository.GetByEmail(request.Email, cancellationToken);
            if (user == null)
            {
                return ApiResponse<LoginResponse>.Fail("Invalid email or password.");
            }

            var activeSessions = await this.sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var activeSession in activeSessions)
            {
                activeSession.Invalidate();
            }

            var refreshToken = this.tokenService.GenerateRefreshToken();
            var expirationMinutes = int.TryParse(this.configuration["Jwt:RefreshTokenExpiryMinutes"], out var mins) ? mins : 10080;
            var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                var validationTime = DateTime.UtcNow;
                user.RecordFailedLoginAttempt(validationTime);

                if (user.LockedOutUntil.HasValue && user.LockedOutUntil > validationTime)
                {
                    await this.userRepository.UpdateAsync(user, cancellationToken);
                    this.logger.LogWarning("User {Email} account locked due to multiple failed login attempts.", request.Email);

                    // await _emailService.SendAccountLockoutEmail(user.Email, LockoutMinutes);
                    return ApiResponse<LoginResponse>.Fail($"Account locked due to multiple failed login attempts. Try again in {user.LockedOutUntil - validationTime} minutes.");
                }

                await this.userRepository.UpdateAsync(user, cancellationToken);
                return ApiResponse<LoginResponse>.Fail("Invalid email or password.");
            }

            var result = user.AttemptLogin(refreshToken, refreshTokenExpiry, DateTime.UtcNow);

            if (!result.IsSuccess)
            {
                return ApiResponse<LoginResponse>.Fail(result.Error!);
            }

            var session = UserSession.Create(
                user.Id,
                refreshToken,
                refreshTokenExpiry,
                request.IpAddress,
                request.UserAgent);

            await this.sessionRepository.Add(session, cancellationToken);

            var accessToken = this.tokenService.GenerateAccessToken(user, session.Id.Value);
            var expiresAt = this.tokenService.GetTokenExpiration(accessToken);

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<LoginResponse>.Ok(
                new LoginResponse(
                accessToken,
                refreshToken),
                "Login successful!");
        }
    }
}
