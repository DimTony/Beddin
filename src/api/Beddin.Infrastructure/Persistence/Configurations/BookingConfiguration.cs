// <copyright file="BookingConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// This class configures the entity framework mapping for the Booking entity, defining how it should be stored in the database, including property conversions, relationships, indexes, and concurrency control. It ensures that the Booking entity is properly mapped to a database table with the necessary constraints and optimizations for efficient querying and data integrity.
    /// </summary>
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(p => p.Id)
               .HasConversion(id => id!.Value, value => new BookingId(value))
               .IsRequired();

            builder.Property(p => p.PropertyId)
              .HasConversion(id => id!.Value, value => new PropertyId(value))
              .IsRequired();

            builder.Property(p => p.ViewerId)
              .HasConversion(id => id!.Value, value => new UserId(value))
              .IsRequired();

            builder.Property(p => p.OwnerId)
              .HasConversion(id => id!.Value, value => new UserId(value))
              .IsRequired();

            builder.Property(b => b.Status)
                .HasConversion<string>() // store as text
                .IsRequired();

            builder.Property(b => b.ScheduledAt)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired();

            builder.HasOne(b => b.Property)
                .WithMany() // or .WithMany(p => p.Bookings) if you add collection
                .HasForeignKey(b => b.PropertyId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes (important for performance)
            builder.HasIndex(b => b.PropertyId);
            builder.HasIndex(b => b.ViewerId);
            builder.HasIndex(b => b.OwnerId);
            builder.HasIndex(b => b.ScheduledAt);
            builder.HasIndex(b => b.Status);

            // Optional: concurrency control
            builder.Property(b => b.UpdatedAt)
                .IsConcurrencyToken();

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
