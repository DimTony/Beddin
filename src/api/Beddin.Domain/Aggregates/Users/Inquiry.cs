// <copyright file="Inquiry.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Users
{
    /// <summary>
    /// Represents an inquiry made by a user regarding a property.
    /// </summary>
    public sealed class Inquiry : AggregateRoot<InquiryId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Inquiry"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        private Inquiry()
        {
        }

        /// <summary>
        /// Gets the identifier of the property associated with this inquiry.
        /// </summary>
        public PropertyId PropertyId { get; private set; } = null!;

        /// <summary>
        /// Gets the identifier of the user who sent this inquiry.
        /// </summary>
        public UserId SenderId { get; private set; } = null!;

        /// <summary>
        /// Gets the identifier of the user who receives this inquiry (agent or owner at time of inquiry).
        /// </summary>
        public UserId RecipientId { get; private set; } = null!; // Agent/owner at time of inquiry

        /// <summary>
        /// Gets the message content of the inquiry.
        /// </summary>
        public string Message { get; private set; } = null!;

        /// <summary>
        /// Gets the status of the inquiry.
        /// </summary>
        public InquiryStatus Status { get; private set; } = InquiryStatus.Unread;

        /// <summary>
        /// Gets the type of the inquiry.
        /// </summary>
        public InquiryType Type { get; private set; } = InquiryType.General;

        /// <summary>
        /// Gets the requested viewing date and time, if any.
        /// </summary>
        public DateTimeOffset? RequestedViewingAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the inquiry was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the inquiry was read, if any.
        /// </summary>
        public DateTimeOffset? ReadAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the inquiry was replied to, if any.
        /// </summary>
        public DateTimeOffset? RepliedAt { get; private set; }

        /// <summary>
        /// Gets the property associated with this inquiry.
        /// </summary>
        public Property Property { get; private set; } = null!;

        /// <summary>
        /// Gets the user who sent this inquiry.
        /// </summary>
        public User Sender { get; private set; } = null!;

        /// <summary>
        /// Gets the user who receives this inquiry.
        /// </summary>
        public User Recipient { get; private set; } = null!;

        /// <summary>
        /// Creates a new <see cref="Inquiry"/> instance.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <param name="senderId">The sender user identifier.</param>
        /// <param name="recipientId">The recipient user identifier.</param>
        /// <param name="message">The inquiry message.</param>
        /// <param name="type">The inquiry type.</param>
        /// <param name="requestedViewingAt">The requested viewing date and time, if any.</param>
        /// <returns>A new <see cref="Inquiry"/> instance.</returns>
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
                CreatedAt = DateTimeOffset.UtcNow,
            };
        }

        /// <summary>
        /// Marks the inquiry as read and sets the read timestamp.
        /// </summary>
        public void MarkAsRead()
        {
            if (this.Status == InquiryStatus.Unread)
            {
                this.Status = InquiryStatus.Read;
                this.ReadAt = DateTimeOffset.UtcNow;
            }
        }

        /// <summary>
        /// Marks the inquiry as replied and sets the replied timestamp.
        /// </summary>
        public void MarkAsReplied()
        {
            this.Status = InquiryStatus.Replied;
            this.RepliedAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Updates the inquiry message.
        /// </summary>
        /// <param name="message">The new message content.</param>
        public void UpdateMessage(string message)
        {
            this.Message = message;
        }
    }
}
