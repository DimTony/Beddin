// <copyright file="UserSession.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Users
{
    /// <summary>
    /// Represents a user session for authentication tracking.
    /// </summary>
    public sealed class UserSession : AggregateRoot<UserSessionId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSession"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        private UserSession()
        {
        }

        /// <summary>
        /// Gets the user identifier for this session.
        /// </summary>
        public UserId UserId { get; private set; } = default!;

        /// <summary>
        /// Gets the hash of the token.
        /// </summary>
        public string TokenHash { get; private set; } = default!;

        /// <summary>
        /// Gets the IP address from which the session was created.
        /// </summary>
        public string? IpAddress { get; private set; }

        /// <summary>
        /// Gets the user agent string from which the session was created.
        /// </summary>
        public string? UserAgent { get; private set; }

        /// <summary>
        /// Gets the date and time when the session was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the session expires.
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the session was invalidated.
        /// </summary>
        public DateTime? InvalidatedAt { get; private set; }

        /// <summary>
        /// Gets the reason why the session was invalidated.
        /// </summary>
        public string? InvalidationReason { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the session is currently active.
        /// </summary>
        public bool IsActive => this.InvalidatedAt is null && DateTime.UtcNow < this.ExpiresAt;

        /// <summary>
        /// Creates a new <see cref="UserSession"/> instance.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token string.</param>
        /// <param name="expiresAt">The expiration date and time.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>A new <see cref="UserSession"/> instance.</returns>
        public static UserSession Create(
            UserId userId,
            string token,
            DateTime expiresAt,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var tokenHash = ComputeHash(token);

            return new UserSession
            {
                Id = UserSessionId.New(),
                UserId = userId,
                TokenHash = tokenHash,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
            };
        }

        /// <summary>
        /// Creates a new <see cref="UserSession"/> instance with a specific session ID.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token string.</param>
        /// <param name="expiresAt">The expiration date and time.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>A new <see cref="UserSession"/> instance.</returns>
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
                TokenHash = ComputeHash(token),
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
            };
        }

        /// <summary>
        /// Computes a hash of the token string.
        /// </summary>
        /// <param name="token">The token to hash.</param>
        /// <returns>The hexadecimal hash string.</returns>
        public static string ComputeHash(string token)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

        /// <summary>
        /// Invalidates the session.
        /// </summary>
        /// <param name="reason">The reason for invalidation.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result Invalidate(string reason = "Logout")
        {
            if (!this.IsActive)
            {
                return Result.Failure("Session is already invalidated.");
            }

            this.InvalidatedAt = DateTime.UtcNow;
            this.InvalidationReason = reason;
            return Result.Success();
        }
    }
}
