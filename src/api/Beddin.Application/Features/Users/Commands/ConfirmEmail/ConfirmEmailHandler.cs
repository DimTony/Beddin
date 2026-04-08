// <copyright file="ConfirmEmailHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Net;
using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.Login;
using Beddin.Domain.Aggregates.Users;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    /// <summary>
    /// Handles the confirmation of a user's email address and activates the user account.
    /// </summary>
    public sealed class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, ApiResponse<LoginResponse>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserSessionRepository sessionRepository;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmEmailHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="sessionRepository">The session repository.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="tokenService">The token service.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public ConfirmEmailHandler(IUserRepository userRepository, IUserSessionRepository sessionRepository, IConfiguration configuration, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.sessionRepository = sessionRepository;
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the confirm email command.
        /// </summary>
        /// <param name="request">The confirm email command request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{LoginResponse}"/> indicating the result of the operation.</returns>
        /// <inheritdoc/>
        public async Task<ApiResponse<LoginResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null || user.EmailConfirmed)
            {
                return ApiResponse<LoginResponse>.Fail("Invalid email or token.");
            }

            var decodedToken = WebUtility.UrlDecode(request.Token);

            var activeSessions = await this.sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
            foreach (var activeSession in activeSessions)
            {
                activeSession.Invalidate();
            }

            var refreshToken = this.tokenService.GenerateRefreshToken();
            var expirationMinutes = int.TryParse(this.configuration["Jwt:RefreshTokenExpiryMinutes"], out var mins) ? mins : 10080;
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
                request.UserAgent);

            await this.sessionRepository.Add(session, cancellationToken);

            var accessToken = this.tokenService.GenerateAccessToken(user, session.Id.Value);

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<LoginResponse>.Ok(
                new LoginResponse(
                accessToken,
                refreshToken),
                "Email confirmed successfully!");
        }
    }
}
