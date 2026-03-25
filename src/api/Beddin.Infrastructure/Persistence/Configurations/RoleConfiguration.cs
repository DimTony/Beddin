using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new RoleId(value));

            builder.Property(u => u.Name).HasMaxLength(256).IsRequired();

            builder.Property(u => u.Description).HasMaxLength(100).IsRequired();

            builder.HasIndex(u => u.Name)
               .IsUnique()
               .HasDatabaseName("ix_roles_name");

            builder.Ignore(p => p.DomainEvents);

        }
    }
}
