using Beddin.Application.Common.DTOs;
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
    public class UserRepository : Repository<User, UserId>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmail(string email, CancellationToken ct = default)
        {
          return  await _dbSet.FirstOrDefaultAsync(
                u => u.Email == email.ToLowerInvariant().Trim(), ct);
        }

        public async Task<User?> GetUserByRefreshToken(string refreshToken, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(
                u => u.RefreshToken == refreshToken, ct);
        }

        public async Task<PagedResult<User>> GetAllUsers(Guid? userId, string? firstName, string? lastName, string? email, Guid? roleId, int pageNumber, int pageSize, CancellationToken ct = default)
        {

            var query = _dbSet.AsQueryable();

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
                query = query.Where(r => r.Email == email);
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
