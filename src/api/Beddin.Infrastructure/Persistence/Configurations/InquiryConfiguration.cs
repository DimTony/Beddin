using Beddin.Domain.Aggregates.Users;
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
    public class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
    {
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
