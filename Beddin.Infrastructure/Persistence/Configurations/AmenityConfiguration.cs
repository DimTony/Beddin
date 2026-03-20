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
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
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
