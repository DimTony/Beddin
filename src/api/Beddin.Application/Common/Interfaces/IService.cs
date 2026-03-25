using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Interfaces
{
    public interface IRateLimitService
    {
        Task<bool> IsAllowedAsync(string key, int maxAttempts, int windowSeconds);
    }
    /// <summary>
    /// Commands that implement this are automatically audit-logged.
    /// </summary>
    public interface IAuditable
    {
        string AuditResource { get; }
        Guid? AuditResourceId { get; }
    }

    /// <summary>
    /// Provides current user context to behaviours and services.
    /// Implemented in API layer using HttpContext.
    /// </summary>
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? IpAddress { get; }
        string? Name { get; }
        string? Email { get; }
        string? UserAgent { get; }
        Guid? SessionId { get; }
        string? Role { get; }
    }
    public interface IAuditLogService
    {
        /// <summary>
        /// Records the initial audit entry before the command executes.
        /// Returns the entry ID so the behaviour can update it after.
        /// </summary>
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
        Task UpdateOutcomeAsync(
            AuditLogId auditEntryId,
            bool succeeded,
            string? failureReason = null,
            CancellationToken ct = default);
    }
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }
        DateTime? DeletedAt { get; }
        void Delete();
        void Restore();
    }

    public interface ITokenService
    {
        string GenerateAccessToken(User user, Guid sessionId, IEnumerable<Claim>? additionalClaims = null);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        DateTime GetTokenExpiration(string token);
        Guid? GetSessionIdFromToken(string token);
    }

    public interface IPasswordService
    {
        string GenerateTemporaryPassword(int length = 12);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body, CancellationToken ct);
        Task SendEmailConfirmationAsync(string email, string confirmationLink);
        Task SendPasswordResetAsync(string email, string resetLink);
        Task SendAccountLockedAsync(string email);
        Task SendWelcomeEmailAsync(string email, string firstName);
    }

    public interface IAuthenticationService
    {
        Task<Result<UserId>> Register(string email, string password);
        //Task<Result<string>> Login(string email, string password);
        ////Task<Result<ExternalLoginInfo>> GetExternalLoginInfo();
        //Task<Result<string>> ExternalLogin(string provider, string providerKey, string email);
        //Task<Result<string>> GenerateRefreshToken(UserId userId);
        //Task<Result<string>> RefreshAccessToken(string refreshToken);
        //Task<Result> RevokeRefreshToken(UserId userId);
        //Task<Result> ChangePassword(UserId userId, string currentPassword, string newPassword);
        //Task<Result> ResetPassword(string email, string resetToken, string newPassword);
        //Task<Result<string>> GeneratePasswordResetToken(string email);
        //Task<Result<string>> GenerateEmailConfirmationToken(UserId userId);
        //Task<Result> ConfirmEmail(UserId userId, string token);
        //Task<bool> IsEmailConfirmed(UserId userId);
        //Task<Result> AssignRole(UserId userId, string role);
        //Task<Result> RemoveRole(UserId userId, string role);
        //Task<IReadOnlyList<string>> GetRoles(UserId userId);
        //Task<Result> AddClaim(UserId userId, string claimType, string claimValue);
        //Task<Result<string>> GenerateTwoFactorToken(UserId userId, string provider);
        //Task<Result> VerifyTwoFactorToken(UserId userId, string provider, string token);
        //Task<Result> EnableTwoFactor(UserId userId);
        //Task<Result> DisableTwoFactor(UserId userId);
        //Task<Result> LockoutUser(UserId userId, DateTimeOffset until);
        //Task<Result> UnlockUser(UserId userId);
        //Task<bool> IsLockedOut(UserId userId);
        //Task<Result> Deactivate(UserId userId);
    }
}
