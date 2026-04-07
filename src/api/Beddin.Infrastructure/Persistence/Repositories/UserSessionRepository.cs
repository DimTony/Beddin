// <copyright file="UserSessionRepository.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Beddin.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implements the <see cref="IUserSessionRepository"/> interface for managing user sessions in the database. This repository provides methods to retrieve, add, update, and invalidate user sessions, as well as to query active sessions and session history for a given user. It uses Entity Framework Core to interact with the underlying data store, ensuring efficient querying and data manipulation while adhering to the repository pattern for clean separation of concerns.
    /// </summary>
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UserSessionRepository(AppDbContext context) => this.context = context;

        /// <inheritdoc/>
        public async Task<UserSession?> GetById(
            Guid sessionId, CancellationToken ct = default) =>
            await this.context.UserSessions.FindAsync([new UserSessionId(sessionId)], ct);

        /// <inheritdoc/>
        public async Task<UserSession?> GetByTokenHash(
            string tokenHash, CancellationToken ct = default) =>
            await this.context.UserSessions
                .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, ct);

        /// <inheritdoc/>
        public async Task<UserSession?> GetActiveSession(
            UserId userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await this.context.UserSessions
                .Where(s =>
                    s.UserId == userId &&
                    s.InvalidatedAt == null &&
                    s.ExpiresAt > now)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserSession>> GetAllActiveSessions(UserId userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await this.context.UserSessions
                .Where(s =>
                    s.UserId == userId &&
                    s.InvalidatedAt == null &&
                    s.ExpiresAt > now)
                .OrderByDescending(s => s.CreatedAt)
                 .ToListAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserSession>> GetSessionHistory(
            UserId userId, CancellationToken ct = default) =>
            await this.context.UserSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(ct);

        /// <inheritdoc/>
        public async Task<PagedResult<UserSession>> GetSessions(
            int pageNumber,
            int pageSize,
            string? userId,
            CancellationToken ct = default)
        {
            var query = this.context.UserSessions.AsQueryable();

            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
            {
                query = query.Where(r => r.UserId == new UserId(userGuid));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return PagedResult<UserSession>.From(
                items, totalCount, pageNumber, pageSize);
        }

        /// <inheritdoc/>
        public async Task Add(UserSession session, CancellationToken ct = default) =>
            await this.context.UserSessions.AddAsync(session, ct);

        /// <inheritdoc/>
        public Task Update(UserSession session, CancellationToken ct = default)
        {
            this.context.UserSessions.Update(session);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task InvalidateAll(
            UserId userId,
            string reason,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var activeSessions = await this.context.UserSessions
                .Where(s =>
                    s.UserId == userId &&
                    s.InvalidatedAt == null &&
                    s.ExpiresAt > now)
                .ToListAsync(ct);

            foreach (var session in activeSessions)
            {
                session.Invalidate(reason);
            }
        }
    }
}
