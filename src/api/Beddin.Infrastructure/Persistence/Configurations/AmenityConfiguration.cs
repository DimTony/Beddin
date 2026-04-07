// <copyright file="AmenityConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Provides the Entity Framework configuration for the <see cref="Amenity"/> entity.
    /// </summary>
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        /// <summary>
        /// Configures the <see cref="Amenity"/> entity type.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, v => new AmenityId(v));

            builder.Property(p => p.Name)
              .IsRequired()
              .HasMaxLength(200);

            builder.Property(p => p.Icon)
                .HasMaxLength(2000);

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
