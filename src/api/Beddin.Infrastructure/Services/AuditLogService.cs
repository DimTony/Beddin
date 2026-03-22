using Azure.Core;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.AuditLog;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Infrastructure.Persistence;
using Beddin.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Beddin.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLogId> RecordAsync(
       Guid? userId,
       string action,
       string resource,
       Guid? resourceId,
       object? oldValue,
       object? newValue,
       string? ipAddress,
       CancellationToken ct = default)
        {
            var entry = AuditLog.Record(
                userId,
                action,
                resource,
                resourceId,
                oldValue,
                newValue,
                ipAddress
            );

            await _context.AuditLogs.AddAsync(entry, ct);
            await _context.SaveChangesAsync(ct);

            return entry.Id;
        }

        public async Task UpdateOutcomeAsync(
            AuditLogId auditEntryId,
            bool succeeded,
            string? failureReason = null,
            CancellationToken ct = default)
        {
            var entry = await _context.AuditLogs
                .FirstOrDefaultAsync(a => a.Id == auditEntryId, ct);

            if (entry is null) return;   // should never happen but guard anyway

            entry.Status = succeeded ? AuditStatus.Succeeded : AuditStatus.Failed;
            entry.FailureReason = failureReason;
            entry.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
    }
}
