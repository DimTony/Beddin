// <copyright file="IService.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Security.Claims;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;

namespace Beddin.Application.Common.Interfaces
{
#pragma warning disable SA1402 // File may only contain a single type
    /// <summary>
    /// Service for managing rate limiting.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public interface IRateLimitService
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Checks if a specific key is allowed based on rate limiting rules.
        /// </summary>
        /// <param name="key">The unique key for the rate limit.</param>
        /// <param name="maxAttempts">Maximum number of attempts allowed.</param>
        /// <param name="windowSeconds">Time window in seconds.</param>
        /// <returns>A task that returns true if allowed, false otherwise.</returns>
        Task<bool> IsAllowedAsync(string key, int maxAttempts, int windowSeconds);
    }

    /// <summary>
    /// Commands that implement this are automatically audit-logged.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Gets the resource being audited.
        /// </summary>
        string AuditResource { get; }

        /// <summary>
        /// Gets the resource identifier being audited.
        /// </summary>
        Guid? AuditResourceId { get; }
    }

    /// <summary>
    /// Provides current user context to behaviours and services.
    /// Implemented in API layer using HttpContext.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the current user's identifier.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Gets the current user's IP address.
        /// </summary>
        string? IpAddress { get; }

        /// <summary>
        /// Gets the current user's name.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets the current user's email.
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Gets the current user's user agent string.
        /// </summary>
        string? UserAgent { get; }

        /// <summary>
        /// Gets the current session identifier.
        /// </summary>
        Guid? SessionId { get; }

        /// <summary>
        /// Gets the current user's role.
        /// </summary>
        string? Role { get; }
    }

    /// <summary>
    /// Service for managing audit logs.
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Records the initial audit entry before the command executes.
        /// Returns the entry ID so the behaviour can update it after.
        /// </summary>
        /// <param name="userId">The user identifier performing the action.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="resource">The resource being affected.</param>
        /// <param name="resourceId">The resource identifier being affected.</param>
        /// <param name="oldValue">The old value before the action.</param>
        /// <param name="newValue">The new value after the action.</param>
        /// <param name="ipAddress">The IP address of the user.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the audit log identifier.</returns>
        Task<AuditLogId> RecordAsync(
            Guid? userId,
            string action,
            string resource,
            Guid? resourceId,
            object? oldValue,
            object? newValue,
            string? ipAddress,
            CancellationToken ct = default);

        /// <summary>
        /// Updates the audit entry after the command completes or fails.
        /// </summary>
        /// <param name="auditEntryId">The audit entry identifier.</param>
        /// <param name="succeeded">Whether the action succeeded.</param>
        /// <param name="failureReason">The reason for failure, if any.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateOutcomeAsync(
            AuditLogId auditEntryId,
            bool succeeded,
            string? failureReason = null,
            CancellationToken ct = default);
    }

    /// <summary>
    /// Interface for entities that support soft deletion.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Gets a value indicating whether the entity is deleted.
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Gets the date and time when the entity was deleted.
        /// </summary>
        DateTime? DeletedAt { get; }

        /// <summary>
        /// Marks the entity as deleted.
        /// </summary>
        void Delete();

        /// <summary>
        /// Restores a deleted entity.
        /// </summary>
        void Restore();
    }

    /// <summary>
    /// Service for managing JWT tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates an access token for a user.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="additionalClaims">Additional claims to include in the token.</param>
        /// <returns>The generated access token.</returns>
        string GenerateAccessToken(User user, Guid sessionId, IEnumerable<Claim>? additionalClaims = null);

        /// <summary>
        /// Generates a refresh token.
        /// </summary>
        /// <returns>The generated refresh token.</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates a token and returns the claims principal.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>The claims principal if valid, null otherwise.</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Gets the expiration date of a token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The expiration date.</returns>
        DateTime GetTokenExpiration(string token);

        /// <summary>
        /// Gets the session identifier from a token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The session identifier if present, null otherwise.</returns>
        Guid? GetSessionIdFromToken(string token);
    }

    /// <summary>
    /// Service for managing passwords.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Generates a temporary password.
        /// </summary>
        /// <param name="length">The length of the password.</param>
        /// <returns>The generated temporary password.</returns>
        string GenerateTemporaryPassword(int length = 12);

        /// <summary>
        /// Hashes a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hash">The hash to verify against.</param>
        /// <returns>True if the password matches the hash, false otherwise.</returns>
        bool VerifyPassword(string password, string hash);
    }

    /// <summary>
    /// Service for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="to">The recipient email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendAsync(string to, string subject, string body, CancellationToken ct);

        /// <summary>
        /// Sends an email confirmation message.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="confirmationLink">The confirmation link.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendEmailConfirmationAsync(string email, string confirmationLink);

        /// <summary>
        /// Sends a password reset email.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="resetLink">The reset link.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendPasswordResetAsync(string email, string resetLink);

        /// <summary>
        /// Sends an account locked notification email.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendAccountLockedAsync(string email);

        /// <summary>
        /// Sends a welcome email to a new user.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="firstName">The user's first name.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendWelcomeEmailAsync(string email, string firstName);
    }

    /// <summary>
    /// Service for managing authentication.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A task that returns a result containing the user identifier if successful.</returns>
        Task<Result<UserId>> Register(string email, string password);
    }

#pragma warning restore SA1402 // File may only contain a single type
}
