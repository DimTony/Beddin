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
    public sealed class LogoutHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutHandler(
            UserManager<ApplicationUser> userManager,
            IUserSessionRepository sessionRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var identityUser = await _userManager.FindByIdAsync(request.UserId);
            if (identityUser == null)
            {
                return Result.Failure("User not found.");
            }

            var userId = new UserId(Guid.Parse(request.UserId));

            if (request.LogoutAllSessions)
            {
                // Invalidate all active sessions for the user
                await _sessionRepository.InvalidateAll(userId, "User logged out from all devices", cancellationToken);
            }
            else if (request.SessionId.HasValue)
            {
                // Invalidate specific session
                var session = await _sessionRepository.GetById(request.SessionId.Value, cancellationToken);
                if (session != null)
                {
                    var invalidationResult = session.Invalidate("User logged out");
                    if (invalidationResult.IsFailure)
                    {
                        return Result.Failure(invalidationResult.Error);
                    }
                    await _sessionRepository.Update(session, cancellationToken);
                }
            }
            else
            {
                // Invalidate the most recent active session
                var activeSession = await _sessionRepository.GetActiveSession(userId, cancellationToken);
                if (activeSession != null)
                {
                    var invalidationResult = activeSession.Invalidate("User logged out");
                    if (invalidationResult.IsFailure)
                    {
                        return Result.Failure(invalidationResult.Error);
                    }
                    await _sessionRepository.Update(activeSession, cancellationToken);
                }
            }

            // Revoke refresh token from Identity user
            identityUser.RefreshToken = null;
            identityUser.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(identityUser);

            // Commit all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }

    public sealed class LogoutAllSessionsHandler : IRequestHandler<LogoutAllSessionsCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutAllSessionsHandler(
            UserManager<ApplicationUser> userManager,
            IUserSessionRepository sessionRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(LogoutAllSessionsCommand request, CancellationToken cancellationToken)
        {
            var identityUser = await _userManager.FindByIdAsync(request.UserId);
            if (identityUser == null)
            {
                return Result.Failure("User not found.");
            }

            var userId = new UserId(Guid.Parse(request.UserId));

            // Invalidate all active sessions
            await _sessionRepository.InvalidateAll(userId, "User logged out from all devices", cancellationToken);

            // Revoke refresh token from Identity user
            identityUser.RefreshToken = null;
            identityUser.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(identityUser);

            // Commit all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
