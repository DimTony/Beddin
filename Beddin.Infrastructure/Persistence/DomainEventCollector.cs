using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;

namespace Beddin.Infrastructure.Persistence
{
    public class EfDomainEventCollector : IDomainEventCollector
    {
        private readonly AppDbContext _context;

        public EfDomainEventCollector(AppDbContext context) => _context = context;

        public IEnumerable<DomainEvent> CollectAndClear()
        {
            var aggregates = _context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var events = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();

            foreach (var aggregate in aggregates)
                aggregate.ClearDomainEvents();

            return events;
        }
    }
}
