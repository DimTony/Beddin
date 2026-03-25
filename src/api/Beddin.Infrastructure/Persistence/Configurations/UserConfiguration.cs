using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .ValueGeneratedNever();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.RoleId)
            .HasConversion(
                id => id.Value,
                value => new RoleId(value))
            .HasColumnName("RoleId")
            .IsRequired();

        builder.HasOne(u => u.Role)
           .WithMany()
           .HasForeignKey(u => u.RoleId)
           .HasPrincipalKey(r => r.Id);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.EmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.EmailConfirmationToken)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(u => u.EmailConfirmationTokenExpiry)
            .IsRequired(false);

        builder.HasIndex(u => u.EmailConfirmationToken)
            .IsUnique()
            .HasFilter("\"EmailConfirmationToken\" IS NOT NULL")
            .HasDatabaseName("IX_Users_EmailConfirmationToken");

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(u => u.RefreshTokenExpiry)
            .IsRequired(false);

        builder.Property(u => u.FailedLoginAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.LockedOutUntil)
            .IsRequired(false);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasFilter("\"Username\" IS NOT NULL")
            .HasDatabaseName("IX_Users_Username");

        builder.HasIndex(u => u.RefreshToken)
            .HasFilter("\"RefreshToken\" IS NOT NULL")
            .HasDatabaseName("IX_Users_RefreshToken");

        builder.HasIndex(u => u.RoleId)
            .HasDatabaseName("IX_Users_RoleId");

        builder.Ignore(u => u.Listings);
        builder.Ignore(u => u.SavedSearches);
        builder.Ignore(u => u.SentInquiries);
        builder.Ignore(u => u.ReceivedInquiries);

        builder.Ignore("_domainEvents");
    }
}