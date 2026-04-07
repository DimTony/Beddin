// <copyright file="UnitOfWork.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Beddin.Application.Common.Interfaces;

namespace Beddin.Infrastructure.Persistence
{
    /// <summary>
    /// Implementation of the Unit of Work pattern.
    /// No changes needed here. SaveChangesAsync is still called — but now only
    /// from DomainEventDispatcherBehaviour, never from inside command handlers.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UnitOfWork(AppDbContext context) => this.context = context;

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            await this.context.SaveChangesAsync(ct);
    }
}
