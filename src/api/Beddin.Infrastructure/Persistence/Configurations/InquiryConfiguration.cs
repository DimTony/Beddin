// <copyright file="InquiryConfiguration.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beddin.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// The <c>InquiryConfiguration</c> class is responsible for configuring the entity mapping for the <see cref="Inquiry"/> entity in the Entity Framework Core context. It implements the <see cref="IEntityTypeConfiguration{TEntity}"/> interface, allowing it to define how the properties of the <see cref="Inquiry"/> entity should be mapped to the database schema. This includes specifying primary keys, property conversions, relationships, indexes, and other constraints. By using this configuration class, you can ensure that the database schema is properly aligned with the domain model for inquiries, facilitating efficient data storage and retrieval while maintaining data integrity and consistency. The configuration also includes specific indexes to optimize common query patterns related to inquiries, such as retrieving unread inquiries for agents or inquiries related to specific properties.
    /// </summary>
    public class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Inquiry> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new InquiryId(value));

            builder.Property(i => i.Message).HasColumnType("text").IsRequired();
            builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(i => i.Type).HasConversion<string>().HasMaxLength(30);

            builder.HasOne(i => i.Property)
                   .WithMany(l => l.Inquiries)
                   .HasForeignKey(i => i.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Sender)
                   .WithMany(u => u.SentInquiries)
                   .HasForeignKey(i => i.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Recipient)
                   .WithMany(u => u.ReceivedInquiries)
                   .HasForeignKey(i => i.RecipientId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Agent inbox: unread inquiries per listing
            builder.HasIndex(i => new { i.RecipientId, i.Status })
                   .HasDatabaseName("ix_inquiries_recipient_status");

            // Buyer's sent inquiries
            builder.HasIndex(i => i.SenderId)
                   .HasDatabaseName("ix_inquiries_sender_id");

            // Inquiries on a specific property
            builder.HasIndex(i => i.PropertyId)
                   .HasDatabaseName("ix_inquiries_property_id");

            // Prevent a buyer from spamming the same property
            builder.HasIndex(i => new { i.PropertyId, i.SenderId })
                   .HasDatabaseName("ix_inquiries_property_sender");
        }
    }
}
