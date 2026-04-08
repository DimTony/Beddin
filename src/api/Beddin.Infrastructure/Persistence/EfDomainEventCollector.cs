// <copyright file="EfDomainEventCollector.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;

namespace Beddin.Infrastructure.Persistence
{
    /// <summary>
    /// The <c>EfDomainEventCollector</c> class is responsible for collecting domain events from entities that implement the <see cref="IHasDomainEvents"/> interface within the Entity Framework Core change tracker. It retrieves all entities that have pending domain events, collects those events into a list, and then clears the domain events from the entities to prevent them from being processed multiple times. This class is typically used in conjunction with a unit of work or a repository pattern to ensure that domain events are properly collected and dispatched after changes to the database context are saved. By implementing the <see cref="IDomainEventCollector"/> interface, it provides a standardized way to gather and manage domain events across the application.
    /// </summary>
    public class EfDomainEventCollector : IDomainEventCollector
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfDomainEventCollector"/> class.
        /// </summary>
        /// <param name="context">The database context used for tracking domain events.</param>
        public EfDomainEventCollector(AppDbContext context) => this.context = context;

        /// <inheritdoc/>
        public IEnumerable<DomainEvent> CollectAndClear()
        {
            var aggregates = this.context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var events = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();

            foreach (var aggregate in aggregates)
            {
                aggregate.ClearDomainEvents();
            }

            return events;
        }
    }
}
