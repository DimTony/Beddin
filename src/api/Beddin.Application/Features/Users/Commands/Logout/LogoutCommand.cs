using Beddin.Application.Common.DTOs;
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
        bool LogoutAllSessions = false
    ) : IRequest<ApiResponse<bool>>;

    public sealed record LogoutAllSessionsCommand(
        string UserId
    ) : IRequest<Result>;
}
