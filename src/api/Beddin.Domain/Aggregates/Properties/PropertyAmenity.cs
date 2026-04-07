// <copyright file="PropertyAmenity.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Properties
{
    /// <summary>
    /// Represents the relationship between a property and an amenity.
    /// </summary>
    public sealed class PropertyAmenity : Entity<PropertyAmenityId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAmenity"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        private PropertyAmenity()
        {
        }

        /// <summary>
        /// Gets the property identifier.
        /// </summary>
        public PropertyId PropertyId { get; private set; } = null!;

        /// <summary>
        /// Gets the amenity identifier.
        /// </summary>
        public AmenityId AmenityId { get; private set; } = null!;

        /// <summary>
        /// Gets the property associated with this amenity.
        /// </summary>
        public Property Property { get; private set; } = null!;

        /// <summary>
        /// Gets the amenity associated with this property.
        /// </summary>
        public Amenity Amenity { get; private set; } = null!;

        /// <summary>
        /// Creates a new <see cref="PropertyAmenity"/> instance.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <param name="amenityId">The amenity identifier.</param>
        /// <returns>A new <see cref="PropertyAmenity"/> instance.</returns>
        public static PropertyAmenity Create(PropertyId propertyId, AmenityId amenityId)
        {
            return new PropertyAmenity
            {
                PropertyId = propertyId,
                AmenityId = amenityId,
            };
        }
    }
}
