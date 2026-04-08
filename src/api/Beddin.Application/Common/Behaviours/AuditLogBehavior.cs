// <copyright file="AuditLogBehavior.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Beddin.Application.Common.Behaviours
{
    /// <summary>
    /// Intercepts every command that implements IAuditable,
    /// captures the result, and writes an audit record.
    /// Runs after validation, before domain event dispatch.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    public class AuditLogBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IAuditLogService auditLog;
        private readonly ICurrentUserService currentUser;
        private readonly IConfiguration config;
        private readonly ILogger<AuditLogBehavior<TRequest, TResponse>> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="auditLog">The audit log service used to record audit entries.</param>
        /// <param name="currentUser">The current user service providing user context.</param>
        /// <param name="config">The application configuration instance.</param>
        /// <param name="logger">The logger used for logging audit behavior events.</param>
        public AuditLogBehavior(
            IAuditLogService auditLog,
            ICurrentUserService currentUser,
            IConfiguration config,
            ILogger<AuditLogBehavior<TRequest, TResponse>> logger)
        {
            this.auditLog = auditLog;
            this.currentUser = currentUser;
            this.config = config;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var auditEnabled = this.config.GetValue<bool>(FeatureFlags.AuditLog);
            if (!auditEnabled || request is not IAuditable auditable)
            {
                return await next();
            }

            var requestName = typeof(TRequest).Name;
            var userId = this.currentUser.UserId;

            this.logger.LogInformation(
                "Executing command {CommandName} by user {UserId}",
                requestName,
                userId);

            var auditEntryId = await this.auditLog.RecordAsync(
                userId: userId,
                action: requestName,
                resource: auditable.AuditResource,
                resourceId: auditable.AuditResourceId,
                oldValue: null,
                newValue: request,
                ipAddress: this.currentUser.IpAddress,
                ct: ct);

            TResponse response;
            try
            {
                response = await next();
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Command {CommandName} failed for user {UserId}",
                    requestName,
                    userId);

                await this.auditLog.UpdateOutcomeAsync(
                    auditEntryId,
                    succeeded: false,
                    failureReason: ex.Message,
                    ct: ct);

                throw;
            }

            var failureReason = ExtractFailureReason(response);
            var succeeded = failureReason is null;

            await this.auditLog.UpdateOutcomeAsync(
                auditEntryId,
                succeeded: succeeded,
                failureReason: failureReason,
                ct: ct);

            this.logger.LogInformation(
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
            if (response is null)
            {
                return null;
            }

            var responseType = typeof(TResponse);

            // Handles Result (non-generic)
            if (response is Result result)
            {
                return result.IsFailure ? result.Error : null;
            }

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
