// <copyright file="ActivateUserCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Admin.Commands.ActivateUser
{
    /// <summary>
    /// The <c>ActivateUserCommand</c> class is responsible for encapsulating the data required to activate a user in the system. It implements the <see cref="IRequest{TResponse}"/> interface from MediatR, allowing it to be handled by a corresponding handler that performs the activation logic. Additionally, it implements the <see cref="IRequiresFeature"/> interface, ensuring that the command can be conditionally executed based on feature flags.
    /// </summary>
    public sealed record ActivateUserCommand(
        string Email,
        string IpAddress,
        string UserAgent) : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        /// <inheritdoc/>
        public string FeatureFlag => FeatureFlags.AdminPanel;
    }
}
