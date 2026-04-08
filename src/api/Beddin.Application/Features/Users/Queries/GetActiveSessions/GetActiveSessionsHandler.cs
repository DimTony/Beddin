// <copyright file="GetActiveSessionsHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetActiveSessions
{
    /// <summary>
    /// Handler definition for getting active sessions of a user using their userId identifier.
    /// </summary>
    public sealed class GetActiveSessionsHandler : IRequestHandler<GetActiveSessionsQuery, Result<IEnumerable<SessionDto>>>
    {
        private readonly IUserSessionRepository sessionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetActiveSessionsHandler"/> class.
        /// </summary>
        /// <param name="sessionRepository">The repository for managing user sessions.</param>
        public GetActiveSessionsHandler(IUserSessionRepository sessionRepository)
        {
            this.sessionRepository = sessionRepository;
        }

        /// <inheritdoc/>
        public async Task<Result<IEnumerable<SessionDto>>> Handle(GetActiveSessionsQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out var userIdGuid))
            {
                return Result.Failure<IEnumerable<SessionDto>>("Invalid user ID format.");
            }

            var userId = new UserId(userIdGuid);
            var sessions = await this.sessionRepository.GetSessionHistory(userId, cancellationToken);

            var sessionDtos = sessions
                .Where(s => s.IsActive) // Only active sessions
                .Select(s => new SessionDto(
                    s.Id.Value,
                    s.UserId.Value,
                    s.IpAddress,
                    s.UserAgent,
                    s.CreatedAt,
                    s.ExpiresAt,
                    s.IsActive))
                .ToList();

            return Result.Success<IEnumerable<SessionDto>>(sessionDtos);
        }
    }
}
