using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beddin.Application.Common.Interfaces
{
    public interface IReadDbContext
    {
        //IQueryable<User> Users { get; }
        IQueryable<Property> Properties { get; }
        IQueryable<Favorite> Favorites { get; }
        IQueryable<Amenity> Amenities { get; }
        IQueryable<PropertyAmenity> PropertyAmenities { get; }
        IQueryable<PropertyImage> PropertyImages { get; }
    }
}
