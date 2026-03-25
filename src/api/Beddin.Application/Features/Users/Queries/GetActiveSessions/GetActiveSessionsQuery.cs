using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetActiveSessions
{
    public sealed record GetActiveSessionsQuery(
        string UserId
    ) : IRequest<Result<IEnumerable<SessionDto>>>;
}
