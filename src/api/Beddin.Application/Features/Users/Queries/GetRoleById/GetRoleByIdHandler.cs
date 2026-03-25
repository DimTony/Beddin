using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetRoleById
{
    public sealed class GetRolesByIdHandler : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        public GetRolesByIdHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<ApiResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null)
            {
                return ApiResponse<RoleDto>.Fail($"Role not found");
            }

            var roleDto = new RoleDto(
                role.Id.Value,
                role.Name,
                role.Description,
                role.CreatedAt
            );

            return ApiResponse<RoleDto>.Ok(roleDto);
        }
    }
}
