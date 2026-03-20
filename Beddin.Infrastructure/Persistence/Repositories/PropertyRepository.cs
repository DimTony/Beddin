using Microsoft.EntityFrameworkCore;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Beddin.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Persistence.Repositories
{
    public class PropertyRepository : Repository<Property, PropertyId>, IPropertyRepository
    {
        public PropertyRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Property>> GetPropertiesByOwner(UserId ownerId, CancellationToken ct = default)
        {
            return await _dbSet
               .Where(p => p.Owner == ownerId)
               .Include(a => a.Amenities)
               .Include(a => a.Images)
               .Include(a => a.Favorites)
               .Include(a => a.Bookings)
               .ToListAsync(ct);
        }
    }
}
