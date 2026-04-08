// <copyright file="AuditLogConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.AuditLog;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// This class configures the entity framework mapping for the AuditLog entity, defining how it should be stored in the database, including property conversions, relationships, indexes, and concurrency control. It ensures that the AuditLog entity is properly mapped to a database table with the necessary constraints and optimizations for efficient querying and data integrity.
    /// </summary>
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(p => p.Id)
               .HasConversion(id => id.Value, value => new AuditLogId(value));

            builder.Property(a => a.UserId).IsRequired();

            builder.Property(a => a.Action).HasMaxLength(100).IsRequired();

            builder.Property(a => a.Resource).HasMaxLength(100).IsRequired();

            builder.Property(a => a.ResourceId);

            builder.Property(a => a.OldValue).HasColumnType("varchar");

            builder.Property(a => a.NewValue).HasColumnType("varchar");

            builder.Property(a => a.IpAddress).HasMaxLength(45);

            builder.Property(a => a.Status)
                .HasConversion<int>()
                .HasMaxLength(20);

            builder.Property(a => a.FailureReason);

            builder.Property(a => a.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(a => a.CompletedAt);

            builder.HasIndex(a => a.UserId);

            builder.HasIndex(a => a.Resource);

            builder.HasIndex(a => a.CreatedAt);

            builder.HasIndex(a => new { a.Resource, a.ResourceId });

            builder.HasIndex(a => a.Status);

            builder.HasIndex(a => new { a.UserId, a.CreatedAt });
        }
    }
}
