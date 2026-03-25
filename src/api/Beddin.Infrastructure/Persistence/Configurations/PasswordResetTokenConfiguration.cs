using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Beddin.Infrastructure.Persistence.Configurations
{
   
    public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new PasswordResetTokenId(value))
                .ValueGeneratedNever();

            builder.Property(p => p.UserId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany() 
                .HasForeignKey(p => p.UserId);

            builder.Property(p => p.Token)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(p => p.Token)
                .IsUnique()
                .HasDatabaseName("IX_PasswordResetTokens_Token");


            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(p => p.ExpiresAt)
                .IsRequired();

            builder.Property(p => p.UsedAt)
                .IsRequired(false);

            builder.Property(p => p.RevokedAt)
                .IsRequired(false);


            builder.Property(p => p.IpAddress)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(p => p.UserAgent)
                .IsRequired()
                .HasMaxLength(512);

            //builder.HasIndex(p => new { p.UserId, p.ExpiresAt });

            builder.HasIndex(p => p.UserId)
                .HasFilter("\"UsedAt\" IS NULL")
                .HasDatabaseName("IX_PasswordResetTokens_ActivePerUser");

            builder.HasIndex(p => p.UserId)
                .HasDatabaseName("IX_PasswordResetTokens_UserId");

            builder.HasIndex(p => p.ExpiresAt)
                .HasDatabaseName("IX_PasswordResetTokens_ExpiresAt");


            builder.HasIndex(p => p.UsedAt)
                .HasDatabaseName("IX_PasswordResetTokens_UsedAt");


            builder.Ignore("_domainEvents");
        }
    }
    
}
