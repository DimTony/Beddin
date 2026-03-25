using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Queries.GetActiveSessions;
using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Queries.GetSessions
{
    public sealed class GetSessionsHandler : IRequestHandler<GetSessionsQuery, PagedResponse<SessionDto>>
    {
        private readonly IUserSessionRepository _sessionRepository;

        public GetSessionsHandler(IUserSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<PagedResponse<SessionDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            var pagedSessions = await _sessionRepository.GetSessions(
                request.Page, 
                request.PageSize, 
                //request.Skip, 
                request.UserId, 
                cancellationToken);

            var sessionDtos = pagedSessions.Items.Select(s => new SessionDto(
                s.Id.Value,
                s.UserId.Value,
                s.IpAddress,
                s.UserAgent,
                s.CreatedAt,
                s.ExpiresAt,
                s.IsActive
            )).ToList();

            return PagedResponse<SessionDto>.Ok(
                sessionDtos, request.Page, request.PageSize, pagedSessions.TotalCount, "Sessions fetched!");
        }
    }
}
