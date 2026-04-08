// <copyright file="CreateRoleCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System;
using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.CreateRole
{
    /// <summary>
    /// Command to create a new role with the specified name and description.
    /// </summary>
    /// <param name="Name">The name of the role to create.</param>
    /// <param name="Description">The description of the role.</param>
    /// <param name="IpAddress">The optional IP address from which the command is issued.</param>
    /// <param name="UserAgent">The optional user agent string of the requester.</param>
    public sealed record CreateRoleCommand(
      string Name,
      string Description,
      string? IpAddress = null,
      string? UserAgent = null)
      : IRequest<ApiResponse<RoleDto>>, IRequiresFeature
    {
        /// <inheritdoc/>
        public string FeatureFlag => FeatureFlags.Authentication;
    }

#pragma warning disable SA1402 // File may only contain a single type
    /// <summary>
    /// A new role with the ID, name, description and date created.
    /// </summary>
    /// <param name="Id">The unique ID of the new role created.</param>
    /// <param name="Name">The name of the role.</param>
    /// <param name="Description">The description of the role.</param>
    /// <param name="CreatedAt">The datetime the role was created.</param>
    public sealed record RoleDto(
#pragma warning restore SA1402 // File may only contain a single type
      Guid Id,
      string Name,
      string Description,
      DateTime CreatedAt);
}
