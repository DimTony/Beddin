// <copyright file="LogoutHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.Logout
{
    /// <summary>
    /// Handles the logout command for a user, invalidating the current session or all sessions as requested.
    /// </summary>
    public sealed class LogoutHandler : IRequestHandler<LogoutCommand, ApiResponse<bool>>
    {
        private readonly IUserSessionRepository sessionRepository;
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserService currentUser;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutHandler"/> class.
        /// </summary>
        /// <param name="sessionRepository">The user session repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="currentUser">The current user service.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public LogoutHandler(
            IUserSessionRepository sessionRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            this.sessionRepository = sessionRepository;
            this.userRepository = userRepository;
            this.currentUser = currentUser;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the logout command, invalidating the user's session(s) as specified.
        /// </summary>
        /// <param name="request">The logout command request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating the result of the logout operation.</returns>
        public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = this.currentUser.UserId;

            if (!userId.HasValue)
            {
                return ApiResponse<bool>.Fail("User not found.");
            }

            var user = await this.userRepository.GetByIdAsync(new UserId(userId.Value), cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("User not found.");
            }

            var sessionId = this.currentUser.SessionId;

            if (request.LogoutAllSessions || !sessionId.HasValue)
            {
                var sessions = await this.sessionRepository.GetAllActiveSessions(user.Id, cancellationToken);
                foreach (var session in sessions)
                {
                    session.Invalidate(request.LogoutAllSessions ? "Logout all sessions" : "Logout misuse detected");
                }
            }
            else if (sessionId.HasValue)
            {
                var session = await this.sessionRepository.GetById(sessionId.Value, cancellationToken);

                if (session != null)
                {
                    session.Invalidate("User logged out");
                    await this.sessionRepository.Update(session, cancellationToken);
                }
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "User is logged out.");
        }
    }
}
