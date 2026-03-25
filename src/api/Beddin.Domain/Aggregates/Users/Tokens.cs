using Beddin.Domain.Common;
using Beddin.Domain.Events;
using System;
using System.Security.Cryptography;

namespace Beddin.Domain.Aggregates.Users
{
    public sealed class PasswordResetToken : AggregateRoot<PasswordResetTokenId>
    {
        public UserId UserId { get; private set; } = default!;
        public string Token { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? UsedAt { get; private set; }   // nullable — unset until consumed
        public DateTime? RevokedAt { get; private set; }
        public string IpAddress { get; private set; } = default!;
        public string UserAgent { get; private set; } = default!;

        private PasswordResetToken() { }

        public static PasswordResetToken Create(
            UserId userId,
            string ipAddress,
            string userAgent)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP address is required.", nameof(ipAddress));

            if (string.IsNullOrWhiteSpace(userAgent))
                throw new ArgumentException("User agent is required.", nameof(userAgent));

            var now = DateTime.UtcNow;
            var secureToken = GenerateSecureToken();

            var resetToken = new PasswordResetToken
            {
                Id = PasswordResetTokenId.New(),
                UserId = userId,
                Token = secureToken,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(10),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            resetToken.RaiseDomainEvent(new PasswordResetTokenCreatedEvent(
                userId,
                secureToken,
                resetToken.ExpiresAt));

            return resetToken;
        }

        public void Use()
        {
            if (UsedAt.HasValue)
                throw new InvalidOperationException("Token has already been used.");

            if (IsExpired())
                throw new InvalidOperationException("Token has expired.");

            UsedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PasswordResetTokenUsedEvent(Id, UserId, UsedAt.Value));
        }

        public void Revoke()
        {
            if (UsedAt.HasValue) return;

            var now = DateTime.UtcNow;
            UsedAt = now;   
            RevokedAt = now;

            RaiseDomainEvent(new PasswordResetTokenRevokedEvent(Id, UserId, RevokedAt.Value));
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

        public bool IsValid() => !UsedAt.HasValue && !IsExpired();

        private static string GenerateSecureToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
    }
}