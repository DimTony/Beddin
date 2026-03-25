using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetRoles
{
    public sealed record RoleDto(
     Guid Id,
     string Name,
     string Description,
     DateTime CreatedAt);
}
