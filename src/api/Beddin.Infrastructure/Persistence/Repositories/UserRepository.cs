// <copyright file="UserRepository.cs" company="Beddin">
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
    /// Provides data access operations specifically for <see cref="User"/> aggregates.
    /// Extends the generic <see cref="Repository{TAggregate, TId}"/> to include
    /// user-specific queries and persistence logic.
    /// </summary>
    public class UserRepository : Repository<User, UserId>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UserRepository(AppDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<User?> GetByEmail(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return await this.dbSet.FirstOrDefaultAsync(
                u => u.Email == normalizedEmail, ct);
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByRefreshToken(string refreshToken, CancellationToken ct = default)
        {
            return await this.dbSet.FirstOrDefaultAsync(
                u => u.RefreshToken == refreshToken, ct);
        }

        /// <inheritdoc/>
        public async Task<PagedResult<User>> GetAllUsers(Guid? userId, string? firstName, string? lastName, string? email, Guid? roleId, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = this.dbSet.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(r => r.Id == new UserId(userId.Value));
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(r => r.FirstName == firstName);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(r => r.LastName == lastName);
            }

            if (!string.IsNullOrEmpty(email))
            {
                var normalizedEmail = email.Trim().ToLowerInvariant();

                query = query.Where(r => r.Email == normalizedEmail);
            }

            if (roleId.HasValue)
            {
                query = query.Where(r => r.RoleId == new RoleId(roleId.Value));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Include(u => u.Role)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return PagedResult<User>.From(
                items, totalCount, pageNumber, pageSize);
        }
    }
}
