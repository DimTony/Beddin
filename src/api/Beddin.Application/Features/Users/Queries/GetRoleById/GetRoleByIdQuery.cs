// <copyright file="GetRoleByIdQuery.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Features.Users.Commands.CreateRole;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetRoleById
{
    /// <summary>
    /// Query to get a role using its unique ID.
    /// </summary>
    /// <param name="RoleId">The unique ID of the role.</param>
    public sealed record GetRoleByIdQuery(RoleId RoleId) : IRequest<ApiResponse<RoleDto>>;
}
