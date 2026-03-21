

namespace Beddin.Domain.Common
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }

    public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents
    {
    }
}
