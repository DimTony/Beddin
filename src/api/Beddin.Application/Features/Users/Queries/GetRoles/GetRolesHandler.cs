// <copyright file="GetRolesHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.CreateRole;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetRoles
{
    /// <summary>
    /// Handles the query to fetch all roles, allowing filtering by the role name.
    /// </summary>
    public class GetRolesHandler
    : IRequestHandler<GetRolesQuery, PagedResponse<RoleDto>>
    {
        private readonly IRoleRepository roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRolesHandler"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        public GetRolesHandler(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResponse<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var pagedRoles = await this.roleRepository.GetAllRoles(request.Page, request.PageSize, request.Skip, request.Name, cancellationToken);

            var roleDtos = pagedRoles.Items.Select(r => new RoleDto(
                r.Id.Value,
                r.Name,
                r.Description,
                r.CreatedAt)).ToList();

            return PagedResponse<RoleDto>.Ok(
                roleDtos, request.Page, request.PageSize, pagedRoles.TotalCount, "Roles fetched!");
        }
    }
}
