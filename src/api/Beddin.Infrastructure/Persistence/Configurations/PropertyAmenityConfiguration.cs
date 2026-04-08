// <copyright file="PropertyAmenityConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="PropertyAmenity"/> entity, including key definitions, property conversions, relationships, and indexes.
    /// </summary>
    public class PropertyAmenityConfiguration : IEntityTypeConfiguration<PropertyAmenity>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<PropertyAmenity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new PropertyAmenityId(value));

            builder.HasKey(pa => new { pa.PropertyId, pa.AmenityId });

            builder.HasIndex(pa => new { pa.PropertyId, pa.AmenityId })
                .IsUnique();

            builder.Property(pa => pa.PropertyId)
                .HasConversion(id => id.Value, value => new PropertyId(value));

            builder.Property(pa => pa.AmenityId)
                .HasConversion(id => id.Value, value => new AmenityId(value));

            builder.HasOne(b => b.Property)
                .WithMany() // or .WithMany(p => p.Bookings) if you add collection
                .HasForeignKey(b => b.PropertyId)
                .HasPrincipalKey(p => p.Id);

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
