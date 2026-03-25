using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Aggregates.Properties
{
    public sealed class Amenity : Entity<AmenityId>
    {
        public string Name { get; private set; } = default!;
        public string? Icon { get; private set; }

        private Amenity() { }

        public static Amenity Create(string name, string? icon = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Amenity name required");

            return new Amenity
            {
                Id = AmenityId.New(),
                Name = name,
                Icon = icon
            };
        }
    }
}
