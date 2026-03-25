using Beddin.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Persistence
{
    /// <summary>
    /// No changes needed here. SaveChangesAsync is still called — but now only
    /// from DomainEventDispatcherBehaviour, never from inside command handlers.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context) => _context = context;

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            await _context.SaveChangesAsync(ct);
    }
}
