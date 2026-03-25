using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Behaviours
{
    /// <summary>
    /// Intercepts every command that implements IAuditable,
    /// captures the result, and writes an audit record.
    /// Runs after validation, before domain event dispatch.
    /// </summary>
    public class AuditLogBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IAuditLogService _auditLog;
        private readonly ICurrentUserService _currentUser;
        private readonly IConfiguration _config;
        private readonly ILogger<AuditLogBehavior<TRequest, TResponse>> _logger;

        public AuditLogBehavior(
            IAuditLogService auditLog,
            ICurrentUserService currentUser,
            IConfiguration config,
            ILogger<AuditLogBehavior<TRequest, TResponse>> logger)
        {
            _auditLog = auditLog;
            _currentUser = currentUser;
            _config = config;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var auditEnabled = _config.GetValue<bool>(FeatureFlags.AuditLog);
            if (!auditEnabled || request is not IAuditable auditable)
                return await next();

            var requestName = typeof(TRequest).Name;
            var userId = _currentUser.UserId;

            _logger.LogInformation(
                "Executing command {CommandName} by user {UserId}",
                requestName, userId);

            var auditEntryId = await _auditLog.RecordAsync(
                userId: userId,
                action: requestName,
                resource: auditable.AuditResource,
                resourceId: auditable.AuditResourceId,
                oldValue: null,
                newValue: request,
                ipAddress: _currentUser.IpAddress,
                ct: ct);

            TResponse response;
            try
            {
                response = await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Command {CommandName} failed for user {UserId}",
                    requestName, userId);

                await _auditLog.UpdateOutcomeAsync(
                    auditEntryId,
                    succeeded: false,
                    failureReason: ex.Message,
                    ct: ct);

                throw;
            }



            var failureReason = ExtractFailureReason(response);
            var succeeded = failureReason is null;

            await _auditLog.UpdateOutcomeAsync(
                auditEntryId,
                succeeded: succeeded,
                failureReason: failureReason,
                ct: ct);

            _logger.LogInformation(
                "Command {CommandName} {Outcome} for resource {Resource} {ResourceId}",
                requestName,
                succeeded ? "succeeded" : "failed",
                auditable.AuditResource,
                auditable.AuditResourceId);

            return response;
        }

        /// <summary>
        /// Extracts the failure reason from a Result or Result&lt;T&gt; response.
        /// Returns null if the response is not a Result type or if it succeeded.
        /// </summary>
        private static string? ExtractFailureReason(TResponse response)
        {
            if (response is null) return null;

            var responseType = typeof(TResponse);

            // Handles Result (non-generic)
            if (response is Result result)
                return result.IsFailure ? result.Error : null;

            // Handles Result<T> (generic) via reflection
            if (responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var isFailureProp = responseType.GetProperty(nameof(Result.IsFailure));
                var errorProp = responseType.GetProperty(nameof(Result.Error));

                var isFailure = (bool)(isFailureProp?.GetValue(response) ?? false);
                return isFailure
                    ? (string?)errorProp?.GetValue(response)
                    : null;
            }

            return null;
        }
    }
}
