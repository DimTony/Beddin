// <copyright file="ConfirmEmailCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.Login;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    /// <summary>
    /// Command to confirm a user's email address using a token.
    /// </summary>
    /// <param name="Email">The email address of the user to confirm.</param>
    /// <param name="Token">The confirmation token sent to the user's email.</param>
    /// <param name="IpAddress">The IP address from which the confirmation is requested (optional).</param>
    /// <param name="UserAgent">The user agent string of the client making the request (optional).</param>
    public sealed record ConfirmEmailCommand(
      string Email,
      string Token,
      string? IpAddress = null,
      string? UserAgent = null) : IRequest<ApiResponse<LoginResponse>>, IRequiresFeature
    {
        /// <inheritdoc/>
        public string FeatureFlag => FeatureFlags.Authentication;
    }
}
