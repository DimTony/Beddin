using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new PropertyId(value));

            builder.Property(p => p.Owner)
                .HasConversion(id => id!.Value, value => new UserId(value))
                .IsRequired();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.PrimaryImage)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Tenor)
                .HasConversion<string>();

            builder.Property(p => p.Type)
                .HasConversion<string>();

            builder.Property(p => p.Status)
                .HasConversion<string>();

            builder.Property(p => p.Listing)
                .HasConversion<string>();

            builder.Property(p => p.Street).HasMaxLength(300);
            builder.Property(p => p.City).HasMaxLength(100);
            builder.Property(p => p.State).HasMaxLength(100);
            builder.Property(p => p.Country).HasMaxLength(100);

            builder.Property(p => p.Latitude);
            builder.Property(p => p.Longitude);

            builder.Property(p => p.Location)
                .HasColumnType("geography(Point, 4326)")   // PostGIS type
                .IsRequired(false);

            builder.Property(p => p.Bedrooms).IsRequired();
            builder.Property(p => p.Bathrooms).HasColumnType("decimal(4,1)").IsRequired();
            builder.Property(p => p.SquareFeet).HasColumnType("decimal(12,2)").IsRequired();
            builder.Property(p => p.LotSize).HasColumnType("decimal(12,2)").IsRequired();
            builder.Property(p => p.YearBuilt).IsRequired(false);

            builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(p => p.IsPublished);
            builder.Property(p => p.IsFeatured);
            builder.Property(p => p.ViewCount);
            builder.Property(p => p.FavoriteCount);

            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt);
            builder.Property(p => p.PublishedAt);

            builder.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey("PropertyImageId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Amenities)
                .WithOne()
                .HasForeignKey("AmentityId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Favorites)
                .WithOne()
                .HasForeignKey("FavoriteId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Bookings)
                .WithOne()
                .HasForeignKey("BookingId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.Owner).HasDatabaseName("ix_properties_owner_id");
            builder.HasIndex(p => p.Status).HasDatabaseName("ix_properties_status");
            builder.HasIndex(p => p.IsPublished).HasDatabaseName("ix_properties_is_published");
            builder.HasIndex(p => p.City).HasDatabaseName("ix_properties_city");
            builder.HasIndex(p => p.Location).HasMethod("GIST").HasDatabaseName("ix_properties_location"); // spatial index

            // Ignore domain events — handled by the dispatcher, not persisted
            builder.Ignore(p => p.DomainEvents);
        }
    }
}
