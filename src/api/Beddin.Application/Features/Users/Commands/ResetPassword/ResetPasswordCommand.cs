// <copyright file="ResetPasswordCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.ResetPassword
{
    /// <summary>
    /// Command to request a password reset.
    /// </summary>
    /// <param name="Email">The email address.</param>
    /// <param name="IpAddress">The IP address of the requester.</param>
    /// <param name="UserAgent">The user agent of the requester.</param>
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
    public sealed record RequestPasswordResetCommand(
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
        string Email,
        string IpAddress,
        string UserAgent)
        : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        /// <summary>
        /// Gets the feature flag that must be enabled for this command.
        /// </summary>
        public string FeatureFlag => FeatureFlags.Authentication;
    }

    /// <summary>
    /// Command to reset a password using a token.
    /// </summary>
    /// <param name="Email">The email address.</param>
    /// <param name="Token">The password reset token.</param>
    /// <param name="NewPassword">The new password.</param>
    public sealed record ResetPasswordCommand(
       string Email,
       string Token,
       string NewPassword)
       : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        /// <summary>
        /// Gets the feature flag that must be enabled for this command.
        /// </summary>
        public string FeatureFlag => FeatureFlags.Authentication;
    }
}
