using Beddin.Domain.Aggregates.AuditLog;
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
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            
            builder.HasKey(a => a.Id);

            builder.Property(p => p.Id)
               .HasConversion(id => id.Value, value => new AuditLogId(value));
            ;
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
