using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.Login
{
    public sealed record LoginCommand(
        string Email,
        string Password,
        string? IpAddress = null,
        string? UserAgent = null
    ) : IRequest<Result<LoginResponse>>;

    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        Guid SessionId
    );
}
