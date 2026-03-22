using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.Logout
{
    public sealed record LogoutCommand(
        string UserId,
        Guid? SessionId = null,
        bool LogoutAllSessions = false
    ) : IRequest<Result>;

    public sealed record LogoutAllSessionsCommand(
        string UserId
    ) : IRequest<Result>;
}
