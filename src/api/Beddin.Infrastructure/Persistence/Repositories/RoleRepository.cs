// <copyright file="RoleRepository.cs" company="Beddin">
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
    /// Provides data access operations specifically for <see cref="Role"/> aggregates.
    /// </summary>
    public class RoleRepository : Repository<Role, RoleId>, IRoleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public RoleRepository(AppDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<Role?> GetByName(string name, CancellationToken ct = default)
        {
            return await this.dbSet.FirstOrDefaultAsync(
                  u => u.Name.ToLower() == name.ToLowerInvariant().Trim(), ct);
        }

        /// <inheritdoc/>
        public async Task<PagedResult<Role>> GetAllRoles(int pageNumber, int pageSize, int skip, string? name, CancellationToken ct = default)
        {
            var query = this.dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

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
