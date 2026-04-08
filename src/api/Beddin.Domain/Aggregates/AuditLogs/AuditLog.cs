// <copyright file="AuditLog.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;
using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.AuditLog
{
    /// <summary>
    /// Represents an audit log entry that captures details about user actions and system events for auditing purposes.
    /// </summary>
    public sealed class AuditLog : AggregateRoot<AuditLogId>
    {
        private AuditLog()
        {
        }

        /// <summary>
        /// Gets or sets the identifier of the user who performed the action.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the action performed by the user.
        /// </summary>
        public string Action { get; set; } = default!;

        /// <summary>
        /// Gets or sets the resource associated with the audit log entry.
        /// </summary>
        public string Resource { get; set; } = default!;

        /// <summary>
        /// Gets or sets the identifier of the resource associated with the audit log entry.
        /// </summary>
        public Guid? ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the old value of the resource before the action was performed.
        /// </summary>
        public string? OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value of the resource after the action was performed.
        /// </summary>
        public string? NewValue { get; set; }

        /// <summary>
        /// Gets or sets the status of the audit log entry.
        /// </summary>
        public AuditStatus Status { get; set; } = AuditStatus.Attempted;

        /// <summary>
        /// Gets or sets the reason for failure if the audit action failed.
        /// </summary>
        public string? FailureReason { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the audit log entry was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the audit log entry was completed.
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the action was performed.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Records a new audit log entry with the specified details.
        /// </summary>
        /// <param name="userId">The identifier of the user who performed the action.</param>
        /// <param name="action">The action performed by the user.</param>
        /// <param name="resource">The resource associated with the audit log entry.</param>
        /// <param name="resourceId">The identifier of the resource associated with the audit log entry.</param>
        /// <param name="oldValue">The old value of the resource before the action was performed.</param>
        /// <param name="newValue">The new value of the resource after the action was performed.</param>
        /// <param name="ipAddress">The IP address from which the action was performed.</param>
        /// <returns>A new <see cref="AuditLog"/> instance representing the recorded audit log entry.</returns>
        public static AuditLog Record(
            Guid? userId,
            string action,
            string resource,
            Guid? resourceId,
            object? oldValue,
            object? newValue,
            string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException("Action is required.", nameof(action));
            }

            var auditLog = new AuditLog
            {
                Id = AuditLogId.New(),
                UserId = userId,
                Action = action,
                Resource = resource,
                ResourceId = resourceId,
                OldValue = oldValue is null ? null : JsonSerializer.Serialize(oldValue),
                NewValue = newValue is null ? null : JsonSerializer.Serialize(newValue, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                }),
                IpAddress = ipAddress,
                Status = AuditStatus.Attempted,
                CreatedAt = DateTime.UtcNow,
            };

            auditLog.RaiseDomainEvent(new AuditLogRecordedEvent(
                auditLog.Id,
                auditLog.Action,
                auditLog.Resource,
                auditLog.ResourceId,
                auditLog.OldValue,
                auditLog.NewValue,
                auditLog.IpAddress));

            return auditLog;
        }

        /// <summary>
        /// Updates the outcome of the audit log entry with the specified status and optional failure reason.
        /// </summary>
        /// <param name="newStatus">The new status to set for the audit log entry.</param>
        /// <param name="failureReason">The reason for failure, if applicable.</param>
        public void UpdateOutcome(AuditStatus newStatus, string? failureReason = null)
        {
            this.Status = newStatus;
            this.FailureReason = failureReason;
            this.CompletedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new AuditLogUpdatedEvent(this.Id, this.Action, this.Resource, this.ResourceId, this.OldValue, this.NewValue, this.IpAddress));
        }
    }
}
