// <copyright file="ResendEmailCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System;
using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.ResendEmail
{
    /// <summary>
    /// Command to resend a confirmation email to a user.
    /// </summary>
    /// <param name="Email">The email address to resend confirmation to.</param>
#pragma warning disable SA1649 // File name should match first type name
    public sealed record ResendConfirmationEmailCommand(
#pragma warning restore SA1649 // File name should match first type name
        string Email)
        : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        /// <summary>
        /// Gets the feature flag that must be enabled for this command.
        /// </summary>
        public string FeatureFlag => FeatureFlags.Authentication;
    }

    /// <summary>
    /// Payload for resending confirmation email.
    /// </summary>
    /// <param name="Email">The email address.</param>
#pragma warning disable SA1402 // File may only contain a single type
    public sealed record ResendConfirmationEmailPayload(
#pragma warning restore SA1402 // File may only contain a single type
        string Email);

    /// <summary>
    /// Data transfer object for user information.
    /// </summary>
    /// <param name="Id">The user identifier.</param>
    /// <param name="FirstName">The user's first name.</param>
    /// <param name="LastName">The user's last name.</param>
    /// <param name="Email">The user's email address.</param>
    /// <param name="Role">The user's role name.</param>
    /// <param name="IsActive">Whether the user is active.</param>
    /// <param name="CreatedAt">The date and time when the user was created.</param>
#pragma warning disable SA1402 // File may only contain a single type
    public sealed record UserDto(
#pragma warning restore SA1402 // File may only contain a single type
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAt);
}
