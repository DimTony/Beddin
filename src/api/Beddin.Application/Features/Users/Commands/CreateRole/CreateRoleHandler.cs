// <copyright file="CreateRoleHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.CreateRole
{
    /// <summary>
    /// Handles the creation of a new role.
    /// </summary>
    public sealed class CreateRoleHandler : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRoleHandler"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public CreateRoleHandler(
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            this.roleRepository = roleRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the creation of a new role based on the specified command.
        /// </summary>
        /// <param name="request">The create role command.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>
        /// An <see cref="ApiResponse{RoleDto}"/> indicating the result of the operation.
        /// </returns>
        public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await this.roleRepository.GetByName(request.Name, cancellationToken);

            if (existingRole != null)
            {
                return ApiResponse<RoleDto>.Fail("Role already exists.");
            }

            var role = Role.Create(request.Name, request.Description);

            await this.roleRepository.AddAsync(role, cancellationToken);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            var roleDto = new RoleDto(
                role.Id.Value,
                role.Name,
                role.Description,
                role.CreatedAt);

            return ApiResponse<RoleDto>.Ok(roleDto, "Role created!");
        }
    }
}
