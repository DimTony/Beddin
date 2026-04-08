// <copyright file="GetActiveSessionsDto.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Features.Users.Queries.GetActiveSessions
{
#pragma warning disable SA1649
    /// <summary>
    /// Represents a user session with details about its state and metadata.
    /// </summary>
    public sealed record SessionDto(

        /// <summary>
        /// Gets the unique identifier for the session.
        /// </summary>
        Guid SessionId,

        /// <summary>
        /// Gets the unique identifier for the user associated with the session.
        /// </summary>
        Guid UserId,

        /// <summary>
        /// Gets the IP address from which the session was created.
        /// </summary>
        string? IpAddress,

        /// <summary>
        /// Gets the user agent string of the client that initiated the session.
        /// </summary>
        string? UserAgent,

        /// <summary>
        /// Gets the date and time when the session was created.
        /// </summary>
        DateTime CreatedAt,

        /// <summary>
        /// Gets the date and time when the session will expire.
        /// </summary>
        DateTime ExpiresAt,

        /// <summary>
        /// Gets a value indicating whether the session is currently active.
        /// </summary>
        bool IsActive);
#pragma warning restore SA1649
}
