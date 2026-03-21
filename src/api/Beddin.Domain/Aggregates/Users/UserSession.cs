using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Aggregates.Users
{
    public sealed class UserSession : AggregateRoot<UserSessionId>
    {
        public UserId UserId { get; private set; } = default!;
        //public string Token { get; private set; } = default!;
        public string TokenHash { get; private set; } = default!;
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? InvalidatedAt { get; private set; }
        public string? InvalidationReason { get; private set; }
        public bool IsActive => InvalidatedAt is null && DateTime.UtcNow < ExpiresAt;

        private UserSession() { }

        public static UserSession Create(
            UserId userId,
            string token,
            DateTime expiresAt,
            string? ipAddress = null,
            string? userAgent = null)
        {
            // Store a hash of the token — never store raw JWTs in DB
            var tokenHash = ComputeHash(token);

            return new UserSession
            {
                Id = UserSessionId.New(),
                UserId = userId,
                //Token = token,
                TokenHash = tokenHash,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
            };
        }

        public static UserSession CreateWithId(
           UserSessionId sessionId,
           UserId userId,
           string token,
           DateTime expiresAt,
           string? ipAddress = null,
           string? userAgent = null)
        {
            return new UserSession
            {
                Id = sessionId,
                UserId = userId,
                //Token = token,
                TokenHash = ComputeHash(token),
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
            };
        }

        public Result Invalidate(string reason = "Logout")
        {
            if (!IsActive)
                return Result.Failure("Session is already invalidated.");

            InvalidatedAt = DateTime.UtcNow;
            InvalidationReason = reason;
            return Result.Success();
        }

        public static string ComputeHash(string token)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }


    }
}
