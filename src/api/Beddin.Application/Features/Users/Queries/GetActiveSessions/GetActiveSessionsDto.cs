using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetActiveSessions
{
    public sealed record SessionDto(
        Guid SessionId,
        string? IpAddress,
        string? UserAgent,
        DateTime CreatedAt,
        DateTime ExpiresAt,
        bool IsActive
    );
}
