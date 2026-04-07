// <copyright file="Tokens.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System;
using System.Security.Cryptography;
using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.Users
{
    /// <summary>
    /// Represents a password reset token for a user.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public sealed class PasswordResetToken : AggregateRoot<PasswordResetTokenId>
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordResetToken"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        private PasswordResetToken()
        {
        }

        /// <summary>
        /// Gets the user identifier this token belongs to.
        /// </summary>
        public UserId UserId { get; private set; } = default!;

        /// <summary>
        /// Gets the token string.
        /// </summary>
        public string Token { get; private set; } = default!;

        /// <summary>
        /// Gets the date and time when the token was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the token expires.
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the token was used.
        /// </summary>
        public DateTime? UsedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the token was revoked.
        /// </summary>
        public DateTime? RevokedAt { get; private set; }

        /// <summary>
        /// Gets the IP address from which the token was requested.
        /// </summary>
        public string IpAddress { get; private set; } = default!;

        /// <summary>
        /// Gets the user agent string from which the token was requested.
        /// </summary>
        public string UserAgent { get; private set; } = default!;

        /// <summary>
        /// Creates a new password reset token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>A new <see cref="PasswordResetToken"/> instance.</returns>
        public static PasswordResetToken Create(
            UserId userId,
            string ipAddress,
            string userAgent)
        {
            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentException("IP address is required.", nameof(ipAddress));
            }

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                throw new ArgumentException("User agent is required.", nameof(userAgent));
            }

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
                UserAgent = userAgent,
            };

            resetToken.RaiseDomainEvent(new PasswordResetTokenCreatedEvent(
                userId,
                secureToken,
                resetToken.ExpiresAt));

            return resetToken;
        }

        /// <summary>
        /// Marks the token as used.
        /// </summary>
        public void Use()
        {
            if (this.UsedAt.HasValue)
            {
                throw new InvalidOperationException("Token has already been used.");
            }

            if (this.IsExpired())
            {
                throw new InvalidOperationException("Token has expired.");
            }

            this.UsedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new PasswordResetTokenUsedEvent(this.Id, this.UserId, this.UsedAt.Value));
        }

        /// <summary>
        /// Revokes the token.
        /// </summary>
        public void Revoke()
        {
            if (this.UsedAt.HasValue)
            {
                return;
            }

            var now = DateTime.UtcNow;
            this.UsedAt = now;
            this.RevokedAt = now;

            this.RaiseDomainEvent(new PasswordResetTokenRevokedEvent(this.Id, this.UserId, this.RevokedAt.Value));
        }

        /// <summary>
        /// Determines whether the token has expired.
        /// </summary>
        /// <returns>True if expired, false otherwise.</returns>
        public bool IsExpired() => DateTime.UtcNow > this.ExpiresAt;

        /// <summary>
        /// Determines whether the token is valid (not used and not expired).
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        public bool IsValid() => !this.UsedAt.HasValue && !this.IsExpired();

        private static string GenerateSecureToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
    }
}
