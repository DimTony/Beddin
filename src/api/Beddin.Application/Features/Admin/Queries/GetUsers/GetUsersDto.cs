using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Queries.GetUsers
{
    public sealed record UserDto(
       Guid UserId,
       string FirstName,
       string LastName,
       string Email,
       string Role,
       bool EmailConfirmed,
       bool IsActive,
       DateTime LastLoginAtAt,
       DateTime CreatedAt
    );
}
