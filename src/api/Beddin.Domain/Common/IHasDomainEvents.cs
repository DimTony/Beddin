// <copyright file="IHasDomainEvents.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Domain.Common
{
    /// <summary>
    /// Represents an entity that has domain events.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public interface IHasDomainEvents
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Gets the collection of domain events associated with the entity.
        /// </summary>
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }

        /// <summary>
        /// Clears all domain events from the entity.
        /// </summary>
        void ClearDomainEvents();
    }
}
