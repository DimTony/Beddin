using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.Logout
{
    public sealed class LogoutHandler : IRequestHandler<LogoutCommand, ApiResponse<bool>>
    {
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutHandler(
            IUserSessionRepository sessionRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            if (!userId.HasValue)
                return ApiResponse<bool>.Fail("User not found.");

            var user = await _userRepository.GetByIdAsync(new UserId(userId.Value), cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("User not found.");
            }

            var sessionId = _currentUser.SessionId;

            if (request.LogoutAllSessions || !sessionId.HasValue)
            {
                var sessions = await _sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
                foreach (var session in sessions)
                    session.Invalidate(request.LogoutAllSessions ? "Logout all sessions" : "Logout misuse detected");
            }
            else if (sessionId.HasValue)
            {
                var session = await _sessionRepository.GetById(sessionId.Value, cancellationToken);

                if (session != null)
                {
                    session.Invalidate("User logged out");
                    await _sessionRepository.Update(session, cancellationToken);

                }
            }
            else
            {

                var activeSession = await _sessionRepository.GetActiveSession(new UserId(userId.Value), cancellationToken);
                if (activeSession != null)
                {
                    activeSession.Invalidate("User logged out");
                    await _sessionRepository.Update(activeSession, cancellationToken);
                }

            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "User is logged out.");
        }
    }
}
