using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Persistence.Repositories
{
    public class ResetPasswordRepository : Repository<PasswordResetToken, PasswordResetTokenId>, IResetPasswordRepository
    {
        public ResetPasswordRepository(AppDbContext context) : base(context) { }

        public async Task<PasswordResetToken?> GetByToken(string token, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(
                  u => u.Token == token.Trim(), ct);
        }

        public async Task<IEnumerable<PasswordResetToken>> GetAllActiveUserTokens(UserId userId, CancellationToken ct = default)
        {
            return  await _dbSet
                .Where(t => t.UserId == userId && t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(ct);
        }

    }
}
