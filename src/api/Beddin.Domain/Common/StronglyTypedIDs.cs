// <copyright file="StronglyTypedIDs.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Domain.Common
{
#pragma warning disable SA1402
    /// <summary>
    /// Represents a strongly typed identifier for an audit log.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
#pragma warning disable SA1649 // File name should match first type name
    public record AuditLogId(Guid Value)
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Creates a new <see cref="AuditLogId"/> with a new GUID.
        /// </summary>
        /// <returns>A new audit log identifier.</returns>
        public static AuditLogId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the audit log identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a user session.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record UserSessionId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="UserSessionId"/> with a new GUID.
        /// </summary>
        /// <returns>A new user session identifier.</returns>
        public static UserSessionId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the user session identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a role.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record RoleId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="RoleId"/> with a new GUID.
        /// </summary>
        /// <returns>A new role identifier.</returns>
        public static RoleId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the role identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a user.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record UserId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="UserId"/> with a new GUID.
        /// </summary>
        /// <returns>A new user identifier.</returns>
        public static UserId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the user identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a password reset token.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record PasswordResetTokenId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="PasswordResetTokenId"/> with a new GUID.
        /// </summary>
        /// <returns>A new password reset token identifier.</returns>
        public static PasswordResetTokenId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the password reset token identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a property.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record PropertyId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="PropertyId"/> with a new GUID.
        /// </summary>
        /// <returns>A new property identifier.</returns>
        public static PropertyId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the property identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a property image.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record PropertyImageId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="PropertyImageId"/> with a new GUID.
        /// </summary>
        /// <returns>A new property image identifier.</returns>
        public static PropertyImageId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the property image identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for an amenity.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record AmenityId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="AmenityId"/> with a new GUID.
        /// </summary>
        /// <returns>A new amenity identifier.</returns>
        public static AmenityId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the amenity identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a property amenity.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record PropertyAmenityId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="PropertyAmenityId"/> with a new GUID.
        /// </summary>
        /// <returns>A new property amenity identifier.</returns>
        public static PropertyAmenityId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the property amenity identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a favorite.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record FavoriteId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="FavoriteId"/> with a new GUID.
        /// </summary>
        /// <returns>A new favorite identifier.</returns>
        public static FavoriteId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the favorite identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a booking.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record BookingId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="BookingId"/> with a new GUID.
        /// </summary>
        /// <returns>A new booking identifier.</returns>
        public static BookingId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the booking identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for an inquiry.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record InquiryId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="InquiryId"/> with a new GUID.
        /// </summary>
        /// <returns>A new inquiry identifier.</returns>
        public static InquiryId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the inquiry identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Represents a strongly typed identifier for a saved search.
    /// </summary>
    /// <param name="Value">The GUID value.</param>
    public record SavedSearchId(Guid Value)
    {
        /// <summary>
        /// Creates a new <see cref="SavedSearchId"/> with a new GUID.
        /// </summary>
        /// <returns>A new saved search identifier.</returns>
        public static SavedSearchId New() => new(Guid.NewGuid());

        /// <summary>
        /// Returns the string representation of the saved search identifier.
        /// </summary>
        /// <returns>The GUID as a string.</returns>
        public override string ToString() => this.Value.ToString();
    }
#pragma warning restore SA1402
}
