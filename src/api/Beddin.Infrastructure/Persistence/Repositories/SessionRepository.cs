using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Beddin.Infrastructure.Persistence.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly AppDbContext _context;

        public UserSessionRepository(AppDbContext context) => _context = context;


        public async Task<UserSession?> GetById(
            Guid sessionId, CancellationToken ct = default) =>
            await _context.UserSessions.FindAsync([sessionId], ct);

        public async Task<UserSession?> GetByTokenHash(
            string tokenHash, CancellationToken ct = default) =>
            await _context.UserSessions
                .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, ct);

        public async Task<UserSession?> GetActiveSession(
            UserId userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await _context.UserSessions
                .Where(s =>
                    s.UserId == userId &&
                    s.InvalidatedAt == null &&
                    s.ExpiresAt > now)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<UserSession>> GetSessionHistory(
            UserId userId, CancellationToken ct = default) =>
            await _context.UserSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(ct);

        public async Task Add(UserSession session, CancellationToken ct = default) =>
            await _context.UserSessions.AddAsync(session, ct);

        public Task Update(UserSession session, CancellationToken ct = default)
        {
            _context.UserSessions.Update(session);
            return Task.CompletedTask;
        }
        public async Task InvalidateAll(
            UserId userId,
            string reason,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var activeSessions = await _context.UserSessions
                .Where(s =>
                    s.UserId == userId &&
                    s.InvalidatedAt == null &&
                    s.ExpiresAt > now)
                .ToListAsync(ct);

            foreach (var session in activeSessions)
                session.Invalidate(reason);
        }
    }
}
