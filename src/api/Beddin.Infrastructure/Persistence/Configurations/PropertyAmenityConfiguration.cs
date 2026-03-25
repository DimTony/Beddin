using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    public class PropertyAmenityConfiguration : IEntityTypeConfiguration<PropertyAmenity>
    {
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
