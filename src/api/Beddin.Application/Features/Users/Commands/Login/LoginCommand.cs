// <copyright file="LoginCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.Login
{
    /// <summary>
    /// Command to login a user.
    /// </summary>
    /// <param name="Email">The user's email address.</param>
    /// <param name="Password">The user's password.</param>
    public sealed record LoginCommand(
        string Email,
        string Password)
        : ApiRequest, IRequest<ApiResponse<LoginResponse>>, IRequiresFeature
    {
        /// <summary>
        /// Gets the feature flag that must be enabled for this command.
        /// </summary>
        public string FeatureFlag => FeatureFlags.Authentication;
    }

    /// <summary>
    /// Response data for a successful login.
    /// </summary>
    /// <param name="AccessToken">The access token.</param>
    /// <param name="RefreshToken">The refresh token.</param>
#pragma warning disable SA1402 // File may only contain a single type
    public sealed record LoginResponse(
#pragma warning restore SA1402 // File may only contain a single type
        string AccessToken,
        string RefreshToken);
}
