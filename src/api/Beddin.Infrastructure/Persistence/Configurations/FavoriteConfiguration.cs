// <copyright file="FavoriteConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// The <c>FavoriteConfiguration</c> class is responsible for configuring the entity mapping for the <see cref="Favorite"/> entity in the Entity Framework Core context. It implements the <see cref="IEntityTypeConfiguration{TEntity}"/> interface, allowing it to define how the properties of the <see cref="Favorite"/> entity should be mapped to the database schema. This includes specifying primary keys, property conversions, relationships, indexes, and other constraints. By using this configuration class, you can ensure that the database schema is properly aligned with the domain model for favorites, facilitating efficient data storage and retrieval while maintaining data integrity and consistency. The configuration also includes specific indexes to optimize common query patterns related to favorites, such as retrieving a user's favorite properties or determining which users have favorited a specific property.
    /// </summary>
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, v => new FavoriteId(v));

            builder.HasIndex(f => new { f.UserId, f.PropertyId })
                .IsUnique();

            builder.HasIndex(f => f.UserId);     // fast "my favorites"
            builder.HasIndex(f => f.PropertyId); // fast "who favorited this"

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
