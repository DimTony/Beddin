// <copyright file="AggregateRoot.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Domain.Common
{
    /// <summary>
    /// Represents the aggregate root of a domain, which is an entity that acts as the entry point to an aggregate and contains domain events.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identifier for the aggregate root.</typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents
    {
    }
}
