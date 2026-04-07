// <copyright file="IDomainEventCollector.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;

namespace Beddin.Application.Common.Interfaces
{
    /// <summary>
    /// Provides a mechanism to collect and clear domain events.
    /// </summary>
    public interface IDomainEventCollector
    {
        /// <summary>
        /// Collects all domain events and clears the internal event store.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{DomainEvent}"/> containing the collected domain events.
        /// </returns>
        IEnumerable<DomainEvent> CollectAndClear();
    }
}
