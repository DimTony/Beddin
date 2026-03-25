using Beddin.Application.Common.DTOs;
using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetRoleById
{
    public sealed record GetRoleByIdQuery(RoleId RoleId) : IRequest<ApiResponse<RoleDto>>;

}
