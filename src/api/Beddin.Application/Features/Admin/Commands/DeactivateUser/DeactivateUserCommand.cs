// <copyright file="DeactivateUserCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Admin.Commands.DeactivateUser
{
    /// <summary>
    /// Command to deactivate a user by email, including audit information.
    /// </summary>
    public sealed record DeactivateUserCommand(

        /// <summary>
        /// The email address of the user to deactivate.
        /// </summary>
        string Email,

        /// <summary>
        /// The IP address from which the deactivation request originated.
        /// </summary>
        string IpAddress,

        /// <summary>
        /// The user agent string of the client making the request.
        /// </summary>
        string UserAgent) : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        /// <inheritdoc/>
        public string FeatureFlag => FeatureFlags.AdminPanel;
    }
}
