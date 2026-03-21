using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
               .HasConversion(id => id!.Value, value => new UserSessionId(value))
               .IsRequired();

            builder.Property(p => p.UserId)
               .HasConversion(id => id!.Value, value => new UserId(value))
               .IsRequired();

            //// Token (⚠️ consider removing this from persistence)
            //builder.Property(x => x.Token)
            //    .HasMaxLength(2048) // JWTs can be long
            //    .IsRequired();

            builder.Property(x => x.TokenHash)
                .HasMaxLength(128) // SHA256 hex = 64 bytes → 128 hex chars
                .IsRequired();

            builder.Property(x => x.IpAddress)
                .HasMaxLength(64);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(512);

            // Dates
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.InvalidatedAt);

            builder.Property(x => x.InvalidationReason)
                .HasMaxLength(256);

            // Ignore computed property
            builder.Ignore(x => x.IsActive);

            // Indexes (VERY important for auth performance)
            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.TokenHash)
                .IsUnique(); // ensures no duplicate tokens

            builder.HasIndex(x => x.ExpiresAt);

            builder.HasIndex(x => x.InvalidatedAt);

            // Optional: composite index for fast active-session lookup
            builder.HasIndex(x => new { x.UserId, x.InvalidatedAt, x.ExpiresAt });

            // Optional: concurrency control
            builder.Property(x => x.ExpiresAt)
                .IsConcurrencyToken();

            builder.Ignore(p => p.DomainEvents);

        }
    }
}
