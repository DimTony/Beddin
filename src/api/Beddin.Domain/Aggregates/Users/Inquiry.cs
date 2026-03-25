using Beddin.Domain.Common;
using System;
using Beddin.Domain.Aggregates.Properties;

namespace Beddin.Domain.Aggregates.Users
{
    public sealed class Inquiry : AggregateRoot<InquiryId>
    {
        public PropertyId PropertyId { get; private set; } = null!;
        public UserId SenderId { get; private set; } = null!;
        public UserId RecipientId { get; private set; } = null!; // Agent/owner at time of inquiry

        public string Message { get; private set; } = null!;
        public InquiryStatus Status { get; private set; } = InquiryStatus.Unread;
        public InquiryType Type { get; private set; } = InquiryType.General;

        // Optional scheduling data
        public DateTimeOffset? RequestedViewingAt { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ReadAt { get; private set; }
        public DateTimeOffset? RepliedAt { get; private set; }

        // Navigation property (single, not collection)
        public Property Property { get; private set; } = null!;
        public User Sender { get; private set; } = null!;
        public User Recipient { get; private set; } = null!;

        // Private constructor for EF Core
        private Inquiry() { }

        // Factory method
        public static Inquiry Create(
            PropertyId propertyId,
            UserId senderId,
            UserId recipientId,
            string message,
            InquiryType type = InquiryType.General,
            DateTimeOffset? requestedViewingAt = null)
        {
            return new Inquiry
            {
                Id = InquiryId.New(),
                PropertyId = propertyId,
                SenderId = senderId,
                RecipientId = recipientId,
                Message = message,
                Type = type,
                RequestedViewingAt = requestedViewingAt,
                Status = InquiryStatus.Unread,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        public void MarkAsRead()
        {
            if (Status == InquiryStatus.Unread)
            {
                Status = InquiryStatus.Read;
                ReadAt = DateTimeOffset.UtcNow;
            }
        }

        public void MarkAsReplied()
        {
            Status = InquiryStatus.Replied;
            RepliedAt = DateTimeOffset.UtcNow;
        }

        public void UpdateMessage(string message)
        {
            Message = message;
        }
    }
}