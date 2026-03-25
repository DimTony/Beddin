using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
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
