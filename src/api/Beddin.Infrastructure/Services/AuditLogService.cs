// <copyright file="AuditLogService.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.AuditLog;
using Beddin.Domain.Common;
using Beddin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Beddin.Infrastructure.Services
{
    /// <summary>
    /// Implements the <see cref="IAuditLogService"/> interface to provide functionality for recording audit logs and updating their outcomes. This service interacts with the database context to persist audit log entries and uses a unit of work pattern to ensure that changes are saved atomically. The service allows for recording detailed information about user actions, including the user ID, action performed, resource affected, old and new values, and IP address. It also provides a method to update the outcome of an audit log entry, marking it as succeeded or failed along with an optional failure reason.
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext context;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public AuditLogService(AppDbContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
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
                ipAddress);

            await this.context.AuditLogs.AddAsync(entry, ct);
            await this.unitOfWork.SaveChangesAsync(ct);

            return entry.Id;
        }

        /// <inheritdoc/>
        public async Task UpdateOutcomeAsync(
            AuditLogId auditEntryId,
            bool succeeded,
            string? failureReason = null,
            CancellationToken ct = default)
        {
            var entry = await this.context.AuditLogs
                .FirstOrDefaultAsync(a => a.Id == auditEntryId, ct);

            if (entry is null)
            {
                return;   // should never happen but guard anyway
            }

            entry.Status = succeeded ? AuditStatus.Succeeded : AuditStatus.Failed;
            entry.FailureReason = failureReason;
            entry.CompletedAt = DateTime.UtcNow;

            await this.unitOfWork.SaveChangesAsync(ct);
        }
    }
}
