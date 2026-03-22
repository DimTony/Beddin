using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Domain.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Beddin.Domain.Aggregates.AuditLog
{
    public sealed class AuditLog : AggregateRoot<AuditLogId>
    
    {
        public Guid? UserId { get; set; }
        public string Action { get; set; } = default!;
        public string Resource { get; set; } = default!;
        public Guid? ResourceId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public AuditStatus Status { get; set; } = AuditStatus.Attempted;
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? IpAddress { get; set; }

        private AuditLog() { }

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
                throw new ArgumentException("Action is required.", nameof(action));

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
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
                IpAddress = ipAddress,
                Status = AuditStatus.Attempted,
                CreatedAt = DateTime.UtcNow
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

        public void UpdateOutcome(AuditStatus newStatus, string? failureReason = null)
        {
            Status = newStatus;
            FailureReason = failureReason;
            CompletedAt = DateTime.UtcNow;

            RaiseDomainEvent(new AuditLogUpdatedEvent(Id, Action, Resource, ResourceId, OldValue, NewValue, IpAddress));
        }

        

    }
}
