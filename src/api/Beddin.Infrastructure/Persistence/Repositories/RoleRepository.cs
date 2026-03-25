using Azure.Core;
using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : Repository<Role, RoleId>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context) { }

        public async Task<Role?> GetByName(string name, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(
                  u => u.Name.ToLower() == name.ToLowerInvariant().Trim(), ct);
        }

        public async Task<PagedResult<Role>> GetAllRoles(int pageNumber, int pageSize, int skip, string? name, CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(r => r.Name.Contains(name));

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(r => r.Name) 
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(ct);

            return PagedResult<Role>.From(
                items, totalCount, pageNumber, pageSize);
        }

    }
}
