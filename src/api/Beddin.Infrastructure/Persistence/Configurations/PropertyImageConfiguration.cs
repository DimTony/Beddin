// <copyright file="PropertyImageConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="PropertyImage"/> entity, including property mappings, relationships, indexes, and constraints to ensure proper database schema generation and data integrity.
    /// </summary>
    public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .HasConversion(id => id.Value, value => new PropertyImageId(value));

            builder.Property(i => i.PropertyId)
                .HasConversion(id => id.Value, value => new PropertyId(value));

            builder.Property(i => i.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.IsPrimary);

            builder.HasOne(b => b.Property)
                .WithMany(l => l.Images)
                .HasForeignKey(b => b.PropertyId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.PropertyId)
               .HasDatabaseName("ix_property_images_listing_id");

            builder.HasIndex(i => new { i.PropertyId, i.IsPrimary })
               .HasFilter("\"IsPrimary\" = true")
               .IsUnique()
               .HasDatabaseName("uix_property_images_one_cover_per_listing");

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
