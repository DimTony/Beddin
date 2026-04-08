// <copyright file="PropertyRepository.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Beddin.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Provides data access methods for the <see cref="Property"/> aggregate, allowing retrieval of properties by owner and including related entities such as amenities, images, favorites, and bookings to support application features that require property information. This repository implements the <see cref="IPropertyRepository"/> interface and extends the base <see cref="Repository{TEntity, TId}"/> class to leverage common data access functionality while providing specific methods for property-related queries.
    /// </summary>
    public class PropertyRepository : Repository<Property, PropertyId>, IPropertyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public PropertyRepository(AppDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Property>> GetPropertiesByOwner(UserId ownerId, CancellationToken ct = default)
        {
            return await this.dbSet
               .Where(p => p.Owner == ownerId)
               .Include(a => a.Amenities)
               .Include(a => a.Images)
               .Include(a => a.Favorites)
               .Include(a => a.Bookings)
               .ToListAsync(ct);
        }
    }
}
