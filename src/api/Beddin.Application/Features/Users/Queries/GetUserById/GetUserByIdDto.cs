using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetUserById
{
    public sealed record UserDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAt
    );
}
