using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Aggregates.Properties
{
    public class PropertyAmenity : Entity<PropertyAmenityId>
    {
        public PropertyId PropertyId { get; private set; } = null!;
        public AmenityId AmenityId { get; private set; } = null!;

        public Property Property { get; private set; } = null!;
        public Amenity Amenity { get; private set; } = null!;

        private PropertyAmenity() { }

        public static PropertyAmenity Create(PropertyId propertyId, AmenityId amenityId)
        {
            return new PropertyAmenity
            {
                PropertyId = propertyId,
                AmenityId = amenityId
            };
        }
    }
}
