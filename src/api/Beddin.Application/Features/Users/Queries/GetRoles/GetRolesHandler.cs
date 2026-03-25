using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetRoles
{
    public class GetRolesHandler
    : IRequestHandler<GetRolesQuery, PagedResponse<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolesHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<PagedResponse<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var pagedRoles = await _roleRepository.GetAllRoles(request.Page, request.PageSize, request.Skip, request.Name, cancellationToken);

            var roleDtos = pagedRoles.Items.Select(r => new RoleDto(
                r.Id.Value,
                r.Name,
                r.Description,
                r.CreatedAt
            )).ToList();

            return PagedResponse<RoleDto>.Ok(
                roleDtos, request.Page, request.PageSize, pagedRoles.TotalCount, "Roles fetched!");
        }
    }
}
