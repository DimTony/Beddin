// <copyright file="GetSessionsHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Queries.GetActiveSessions;
using MediatR;

namespace Beddin.Application.Features.Admin.Queries.GetSessions
{
    /// <summary>
    /// Handles the retrieval of paged user session data for administrative queries.
    /// </summary>
    public sealed class GetSessionsHandler : IRequestHandler<GetSessionsQuery, PagedResponse<SessionDto>>
    {
        private readonly IUserSessionRepository sessionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSessionsHandler"/> class.
        /// </summary>
        /// <param name="sessionRepository">The repository for managing user sessions.</param>
        public GetSessionsHandler(IUserSessionRepository sessionRepository)
        {
            this.sessionRepository = sessionRepository;
        }

        /// <summary>
        /// Handles the request to retrieve a paged list of user sessions.
        /// </summary>
        /// <param name="request">The query containing paging and filter parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A paged response containing session DTOs.</returns>
        public async Task<PagedResponse<SessionDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            var pagedSessions = await this.sessionRepository.GetSessions(
                request.Page,
                request.PageSize,
                request.UserId,
                cancellationToken);

            var sessionDtos = pagedSessions.Items.Select(s => new SessionDto(
                s.Id.Value,
                s.UserId.Value,
                s.IpAddress,
                s.UserAgent,
                s.CreatedAt,
                s.ExpiresAt,
                s.IsActive)).ToList();

            return PagedResponse<SessionDto>.Ok(
                sessionDtos, request.Page, request.PageSize, pagedSessions.TotalCount, "Sessions fetched!");
        }
    }
}
