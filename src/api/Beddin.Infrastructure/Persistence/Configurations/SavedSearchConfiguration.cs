using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    public class SavedSearchConfiguration : IEntityTypeConfiguration<SavedSearch>
    {
        public void Configure(EntityTypeBuilder<SavedSearch> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new SavedSearchId(value));

            builder.Property(p => p.UserId)
             .HasConversion(id => id.Value, value => new UserId(value))
             .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(u => u.SavedSearches)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.PropertyType)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.TransactionType)
                .HasConversion<string>();

            builder.Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.State)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Street)
                .HasMaxLength(200);

            builder.Property(x => x.MinPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.MaxPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.MinBedrooms);
            builder.Property(x => x.MaxBedrooms);

            builder.Property(x => x.MinSizeInSqm)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.MaxSizeInSqm)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.AlertEnabled)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.LastAlertSentAt);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.AlertEnabled);
            builder.HasIndex(x => new { x.City, x.State, x.Country });
            builder.HasIndex(x => new { x.MinPrice, x.MaxPrice });

            builder.HasOne(s => s.User)
              .WithMany(u => u.SavedSearches)
              .HasForeignKey(s => s.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.UserId)
               .HasDatabaseName("ix_saved_searches_user_id");

            builder.HasIndex(s => s.AlertEnabled)
               .HasFilter("\"AlertEnabled\" = true")
               .HasDatabaseName("ix_saved_searches_alert_enabled");

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
