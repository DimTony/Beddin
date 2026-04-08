// <copyright file="Repository.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Beddin.Infrastructure.Persistence
{
    /// <summary>
    /// Provides a generic implementation of the repository pattern for managing
    /// aggregate root entities. This class encapsulates the logic required to
    /// perform data access operations such as adding, retrieving, updating,
    /// and deleting aggregates, while abstracting the underlying data store.
    /// </summary>
    /// <typeparam name="TAggregate">
    /// The type of the aggregate root entity. Must inherit from <see cref="AggregateRoot{TId}"/>.
    /// </typeparam>
    /// <typeparam name="TId">
    /// The type of the identifier used by the aggregate.
    /// </typeparam>
    public class Repository<TAggregate, TId> : IRepository<TAggregate, TId>
       where TAggregate : AggregateRoot<TId>
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// The database context used for data access operations.
        /// </summary>
        protected readonly AppDbContext context;

        /// <summary>
        /// The set of aggregate root entities managed by this repository.
        /// </summary>
        protected readonly DbSet<TAggregate> dbSet;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TAggregate, TId}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public Repository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TAggregate>();
        }

        /// <inheritdoc/>
        public virtual async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken ct = default) =>
            await this.dbSet.FindAsync(new object?[] { id }, ct);

        /// <inheritdoc/>
        public virtual async Task AddAsync(TAggregate aggregate, CancellationToken ct = default) =>
            await this.dbSet.AddAsync(aggregate, ct);

        /// <inheritdoc/>
        public virtual Task UpdateAsync(TAggregate aggregate, CancellationToken ct = default)
        {
            this.dbSet.Update(aggregate);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task DeleteAsync(TAggregate aggregate, CancellationToken ct = default)
        {
            this.dbSet.Remove(aggregate);
            return Task.CompletedTask;
        }
    }
}
