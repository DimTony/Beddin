using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using Beddin.Domain.Aggregates.Users;
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
    }
}
