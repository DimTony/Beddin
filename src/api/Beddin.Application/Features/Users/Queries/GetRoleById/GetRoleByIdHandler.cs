// <copyright file="GetRoleByIdHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.CreateRole;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetRoleById
{
    /// <summary>
    /// Handles the query to fetch a role using its unique ID.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public sealed class GetRolesByIdHandler : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDto>>
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly IRoleRepository roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRolesByIdHandler"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        public GetRolesByIdHandler(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await this.roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null)
            {
                return ApiResponse<RoleDto>.Fail($"Role not found");
            }

            var roleDto = new RoleDto(
                role.Id.Value,
                role.Name,
                role.Description,
                role.CreatedAt);

            return ApiResponse<RoleDto>.Ok(roleDto);
        }
    }
}
