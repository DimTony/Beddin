// <copyright file="DeleteUserCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.Login;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.DeleteUser
{
    /// <summary>
    /// Command to delete a user by email address.
    /// </summary>
    /// <param name="Email">The email address of the user to delete.</param>
    /// <param name="IpAddress">The IP address from which the request originated (optional).</param>
    /// <param name="UserAgent">The user agent string of the client making the request (optional).</param>
    public sealed record DeleteUserCommand(
       string Email,
       string? IpAddress = null,
       string? UserAgent = null) : IRequest<ApiResponse<LoginResponse>>, IRequiresFeature
    {
        /// <inheritdoc/>
        public string FeatureFlag => FeatureFlags.Authentication;
    }
}
