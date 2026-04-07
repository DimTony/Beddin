// <copyright file="RoleConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Role entity, defining how the Role aggregate is mapped to the database schema. This includes specifying primary keys, property configurations (such as data types and constraints), indexes, and any relationships with other entities. The configuration ensures that the Role entity is correctly represented in the database and that data integrity is maintained according to the domain model's requirements.
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <inheritdoc/>
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
