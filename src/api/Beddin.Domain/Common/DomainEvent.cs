// <copyright file="DomainEvent.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using MediatR;

namespace Beddin.Domain.Common
{
    /// <summary>
    /// Represents the base class for all domain events.
    /// </summary>
    public abstract record DomainEvent : INotification
    {
        /// <summary>
        /// Gets the unique identifier for the event.
        /// </summary>
        public Guid EventId { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the UTC date and time when the event occurred.
        /// </summary>
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }
}
