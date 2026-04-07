// <copyright file="IReadDbContext.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.AuditLog;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Aggregates.Users;

namespace Beddin.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a read-only database context for querying application data.
    /// </summary>
    public interface IReadDbContext
    {
        /// <summary>
        /// Gets the queryable collection of audit logs.
        /// </summary>
        IQueryable<AuditLog> AuditLogs { get; }

        /// <summary>
        /// Gets the queryable collection of users.
        /// </summary>
        IQueryable<User> Users { get; }

        /// <summary>
        /// Gets the queryable collection of password reset tokens.
        /// </summary>
        IQueryable<PasswordResetToken> PasswordResetTokens { get; }

        /// <summary>
        /// Gets the queryable collection of roles.
        /// </summary>
        IQueryable<Role> Roles { get; }

        /// <summary>
        /// Gets the queryable collection of saved searches.
        /// </summary>
        IQueryable<SavedSearch> SavedSearches { get; }

        /// <summary>
        /// Gets the queryable collection of user sessions.
        /// </summary>
        IQueryable<UserSession> UserSessions { get; }

        /// <summary>
        /// Gets the queryable collection of properties.
        /// </summary>
        IQueryable<Property> Properties { get; }

        /// <summary>
        /// Gets the queryable collection of favorites.
        /// </summary>
        IQueryable<Favorite> Favorites { get; }

        /// <summary>
        /// Gets the queryable collection of amenities.
        /// </summary>
        IQueryable<Amenity> Amenities { get; }

        /// <summary>
        /// Gets the queryable collection of property amenities.
        /// </summary>
        IQueryable<PropertyAmenity> PropertyAmenities { get; }

        /// <summary>
        /// Gets the queryable collection of property images.
        /// </summary>
        IQueryable<PropertyImage> PropertyImages { get; }

        /// <summary>
        /// Gets the queryable collection of inquiries.
        /// </summary>
        IQueryable<Inquiry> Inquiries { get; }
    }
}
