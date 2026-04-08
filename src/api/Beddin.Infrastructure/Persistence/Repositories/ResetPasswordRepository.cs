// <copyright file="ResetPasswordRepository.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Beddin.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implements the <see cref="IResetPasswordRepository"/> interface to provide data access operations for password reset tokens.
    /// </summary>
    public class ResetPasswordRepository : Repository<PasswordResetToken, PasswordResetTokenId>, IResetPasswordRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ResetPasswordRepository(AppDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<PasswordResetToken?> GetByToken(string token, CancellationToken ct = default)
        {
            return await this.dbSet.FirstOrDefaultAsync(
                  u => u.Token == token.Trim(), ct);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PasswordResetToken>> GetAllActiveUserTokens(UserId userId, CancellationToken ct = default)
        {
            return await this.dbSet
                .Where(t => t.UserId == userId && t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(ct);
        }
    }
}
