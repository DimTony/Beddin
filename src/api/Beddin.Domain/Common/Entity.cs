// <copyright file="Entity.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Domain.Common
{
    /// <summary>
    /// Represents a base entity with an identifier and domain event support.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    public abstract class Entity<TId>
    {
        private readonly List<DomainEvent> domainEvents = new();

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public TId Id { get; protected set; } = default!;

        /// <summary>
        /// Gets a value indicating whether the entity is deleted.
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Gets the date and time when the entity was deleted.
        /// </summary>
        public DateTime? DeletedAt { get; private set; }

        /// <summary>
        /// Gets the collection of domain events raised by the entity.
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => this.domainEvents.AsReadOnly();

        /// <summary>
        /// Clears all domain events from the entity.
        /// </summary>
        public void ClearDomainEvents() =>
            this.domainEvents.Clear();

        /// <summary>
        /// Marks the entity as deleted and sets the deletion timestamp.
        /// </summary>
        public void Delete()
        {
            this.IsDeleted = true;
            this.DeletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Restores the entity by marking it as not deleted and clearing the deletion timestamp.
        /// </summary>
        public void Restore()
        {
            this.IsDeleted = false;
            this.DeletedAt = null;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return EqualityComparer<TId>.Default.Equals(this.Id, other.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode() =>
            HashCode.Combine(this.GetType(), this.Id);

        /// <summary>
        /// Raises a domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event to raise.</param>
        protected void RaiseDomainEvent(DomainEvent domainEvent) =>
            this.domainEvents.Add(domainEvent);
    }
}
