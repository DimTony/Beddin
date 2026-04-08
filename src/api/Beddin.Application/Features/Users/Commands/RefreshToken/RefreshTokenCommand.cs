// <copyright file="RefreshTokenCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.RefreshToken
{
    /// <summary>
    /// Command to refresh an access token using a refresh token.
    /// </summary>
    /// <param name="RefreshToken">The refresh token.</param>
    /// <param name="IpAddress">The IP address of the client.</param>
    /// <param name="UserAgent">The user agent string of the client.</param>
    public sealed record RefreshTokenCommand(
        string RefreshToken,
        string? IpAddress = null,
        string? UserAgent = null)
        : IRequest<ApiResponse<RefreshTokenResponse>>;

    /// <summary>
    /// Response containing new tokens after a successful refresh.
    /// </summary>
    /// <param name="AccessToken">The new access token.</param>
    /// <param name="RefreshToken">The new refresh token.</param>
#pragma warning disable SA1402 // File may only contain a single type
    public sealed record RefreshTokenResponse(
#pragma warning restore SA1402 // File may only contain a single type
        string AccessToken,
        string RefreshToken);
}
