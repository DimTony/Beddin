// <copyright file="RefreshTokenHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Beddin.Application.Features.Users.Commands.RefreshToken
{
    /// <summary>
    /// Handler for the <see cref="RefreshTokenCommand"/>.
    /// </summary>
    public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<RefreshTokenResponse>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserSessionRepository sessionRepository;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="sessionRepository">The session repository.</param>
        /// <param name="tokenService">The token service.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="configuration">The configuration.</param>
        public RefreshTokenHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.sessionRepository = sessionRepository;
            this.tokenService = tokenService;
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetUserByRefreshToken(request.RefreshToken, cancellationToken);

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
            var session = await this.sessionRepository.GetByTokenHash(tokenHash, cancellationToken);

            if (session == null)
            {
                return ApiResponse<RefreshTokenResponse>.Fail("Invalid refresh token.");
            }

            if (!session.IsActive)
            {
                var allSessions = await this.sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
                foreach (var s in allSessions)
                {
                    s.Invalidate("Refresh token reuse detected");
                }

                await this.unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<RefreshTokenResponse>.Fail("Invalid refresh token.");
            }

            var activeSessions = await this.sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var activeSession in activeSessions)
            {
                activeSession.Invalidate("Token refreshed");
            }

            var newRefreshToken = this.tokenService.GenerateRefreshToken();
            var expirationMinutes = int.TryParse(this.configuration["Jwt:RefreshTokenExpiryMinutes"], out var mins) ? mins : 10080;
            var newRefreshTokenExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);

            user.SetRefreshToken(newRefreshToken, newRefreshTokenExpiry);

            var newSession = UserSession.Create(
                user.Id,
                newRefreshToken,
                newRefreshTokenExpiry,
                request.IpAddress,
                request.UserAgent);

            await this.sessionRepository.Add(newSession, cancellationToken);

            var accessToken = this.tokenService.GenerateAccessToken(user, newSession.Id.Value);

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<RefreshTokenResponse>.Ok(new RefreshTokenResponse(
                accessToken,
                newRefreshToken));
        }
    }
}
