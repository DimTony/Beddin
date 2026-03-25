namespace Beddin.Domain.Common
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; } = default!;
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(DomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() =>
            _domainEvents.Clear();

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode() =>
            HashCode.Combine(GetType(), Id);
    }
}
