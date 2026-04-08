// <copyright file="GetRolesQuery.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Features.Users.Commands.CreateRole;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetRoles
{
    /// <summary>
    /// Get all roles. Allows filtering by name.
    /// </summary>
    /// <param name="Name">The name of the role to filter by.</param>
    public record GetRolesQuery(
       string? Name) : PagedQuery, IRequest<PagedResponse<RoleDto>>;
}
