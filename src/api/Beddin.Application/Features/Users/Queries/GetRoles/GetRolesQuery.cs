using Beddin.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetRoles
{
    public record GetRolesQuery(
       string? Name) : PagedQuery, IRequest<PagedResponse<RoleDto>>;

   
}
