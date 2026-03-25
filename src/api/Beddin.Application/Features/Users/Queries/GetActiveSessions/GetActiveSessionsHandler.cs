using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Queries.GetActiveSessions
{
    public sealed class GetActiveSessionsHandler : IRequestHandler<GetActiveSessionsQuery, Result<IEnumerable<SessionDto>>>
    {
        private readonly IUserSessionRepository _sessionRepository;

        public GetActiveSessionsHandler(IUserSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<IEnumerable<SessionDto>>> Handle(GetActiveSessionsQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out var userIdGuid))
            {
                return Result.Failure<IEnumerable<SessionDto>>("Invalid user ID format.");
            }

            var userId = new UserId(userIdGuid);
            var sessions = await _sessionRepository.GetSessionHistory(userId, cancellationToken);

            var sessionDtos = sessions
                .Where(s => s.IsActive) // Only active sessions
                .Select(s => new SessionDto(
                    s.Id.Value,
                    s.UserId.Value,
                    s.IpAddress,
                    s.UserAgent,
                    s.CreatedAt,
                    s.ExpiresAt,
                    s.IsActive
                ))
                .ToList();

            return Result.Success<IEnumerable<SessionDto>>(sessionDtos);
        }
    }
}
