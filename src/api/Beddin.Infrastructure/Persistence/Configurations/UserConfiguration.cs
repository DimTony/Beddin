using Beddin.Domain.Aggregates.Users;
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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new UserId(value));

            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();

            builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();

            builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();

            builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

            builder.Property(u => u.CreatedAt).IsRequired();

            builder.Property(u => u.UpdatedAt).IsRequired();

            builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("ix_users_email");

            // Filtered index for active users — most queries filter on this
            builder.HasIndex(u => u.IsActive)
                   .HasFilter("\"IsActive\" = true")
                   .HasDatabaseName("ix_users_active");

            builder.HasIndex(u => u.Role)
                   .HasDatabaseName("ix_users_role");

            builder.Ignore(p => p.DomainEvents);

        }

    }
}
