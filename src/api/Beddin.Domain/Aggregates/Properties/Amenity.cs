// <copyright file="Amenity.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Properties
{
    /// <summary>
    /// Represents an amenity associated with a property, such as a swimming pool, gym, or parking space.
    /// </summary>
    public sealed class Amenity : Entity<AmenityId>
    {
        private Amenity()
        {
        }

        /// <summary>
        /// Gets the name of the amenity.
        /// </summary>
        public string Name { get; private set; } = default!;

        /// <summary>
        /// Gets the optional icon representing the amenity.
        /// </summary>
        public string? Icon { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Amenity"/> instance with the specified name and optional icon.
        /// </summary>
        /// <param name="name">The name of the amenity.</param>
        /// <param name="icon">The optional icon representing the amenity.</param>
        /// <returns>A new <see cref="Amenity"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
        public static Amenity Create(string name, string? icon = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Amenity name required");
            }

            return new Amenity
            {
                Id = AmenityId.New(),
                Name = name,
                Icon = icon,
            };
        }
    }
}
