// <copyright file="Enums.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Domain.Common
{
#pragma warning disable SA1402
    /// <summary>
    /// Represents the status of an audit operation.
    /// </summary>
    public enum AuditStatus
    {
        /// <summary>
        /// The audit was attempted.
        /// </summary>
        Attempted = 0,

        /// <summary>
        /// The audit succeeded.
        /// </summary>
        Succeeded = 1,

        /// <summary>
        /// The audit failed.
        /// </summary>
        Failed = 2,
    }

    /// <summary>
    /// Specifies the role of a user in the system.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Buyer role.
        /// </summary>
        Buyer = 0,

        /// <summary>
        /// Owner role.
        /// </summary>
        Owner = 1,

        /// <summary>
        /// Admin role.
        /// </summary>
        Admin = 2,
    }

    /// <summary>
    /// Represents the status of an inquiry.
    /// </summary>
    public enum InquiryStatus
    {
        /// <summary>
        /// Inquiry is unread.
        /// </summary>
        Unread = 0,

        /// <summary>
        /// Inquiry has been read.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Inquiry has been replied to.
        /// </summary>
        Replied = 2,

        /// <summary>
        /// Inquiry is archived.
        /// </summary>
        Archived = 3,
    }

    /// <summary>
    /// Specifies the type of inquiry.
    /// </summary>
    public enum InquiryType
    {
        /// <summary>
        /// General inquiry.
        /// </summary>
        General = 0,

        /// <summary>
        /// Request for viewing.
        /// </summary>
        ViewingRequest = 1,

        /// <summary>
        /// Price negotiation inquiry.
        /// </summary>
        PriceNegotiation = 2,
    }

    /// <summary>
    /// Represents the type of transaction.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Sale transaction.
        /// </summary>
        Sale = 0,

        /// <summary>
        /// Rent transaction.
        /// </summary>
        Rent = 1,

        /// <summary>
        /// Short let transaction.
        /// </summary>
        ShortLet = 2,

        /// <summary>
        /// Joint venture transaction.
        /// </summary>
        JointVenture = 3,
    }

    /// <summary>
    /// Specifies the frequency of alerts.
    /// </summary>
    public enum AlertFrequency
    {
        /// <summary>
        /// Immediate alert.
        /// </summary>
        Immediate = 0,

        /// <summary>
        /// Daily alert.
        /// </summary>
        Daily = 1,

        /// <summary>
        /// Weekly alert.
        /// </summary>
        Weekly = 2,
    }

    /// <summary>
    /// Represents the type of property.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// House property.
        /// </summary>
        House = 0,

        /// <summary>
        /// Apartment property.
        /// </summary>
        Apartment = 1,

        /// <summary>
        /// Condo property.
        /// </summary>
        Condo = 2,

        /// <summary>
        /// Townhouse property.
        /// </summary>
        Townhouse = 3,

        /// <summary>
        /// Land property.
        /// </summary>
        Land = 4,

        /// <summary>
        /// Commercial property.
        /// </summary>
        Commercial = 5,
    }

    /// <summary>
    /// Specifies the tenor of a property.
    /// </summary>
    public enum PropertyTenor
    {
        /// <summary>
        /// Daily tenor.
        /// </summary>
        Daily = 0,

        /// <summary>
        /// Weekly tenor.
        /// </summary>
        Weekly = 1,

        /// <summary>
        /// Bi-weekly tenor.
        /// </summary>
        BiWeekly = 2,

        /// <summary>
        /// Monthly tenor.
        /// </summary>
        Monthly = 3,

        /// <summary>
        /// Annual tenor.
        /// </summary>
        Annually = 4,

        /// <summary>
        /// Bi-annual tenor.
        /// </summary>
        BiAnnually = 5,

        /// <summary>
        /// Outright purchase.
        /// </summary>
        Outright = 6,
    }

    /// <summary>
    /// Represents the type of listing.
    /// </summary>
    public enum ListingType
    {
        /// <summary>
        /// For sale listing.
        /// </summary>
        ForSale = 0,

        /// <summary>
        /// For rent listing.
        /// </summary>
        ForRent = 1,
    }

    /// <summary>
    /// Specifies the status of a property.
    /// </summary>
    public enum PropertyStatus
    {
        /// <summary>
        /// Draft status.
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Active status.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Pending status.
        /// </summary>
        Pending = 2,

        /// <summary>
        /// Sold status.
        /// </summary>
        Sold = 3,

        /// <summary>
        /// Inactive status.
        /// </summary>
        Inactive = 4,

        /// <summary>
        /// Withdrawn status.
        /// </summary>
        Withdrawn = 5,

        /// <summary>
        /// Expired status.
        /// </summary>
        Expired = 6,

        /// <summary>
        /// Deleted status.
        /// </summary>
        Deleted = 7,
    }

    /// <summary>
    /// Represents the category of an amenity.
    /// </summary>
    public enum AmenityCategory
    {
        /// <summary>
        /// Interior amenity.
        /// </summary>
        Interior = 0,

        /// <summary>
        /// Exterior amenity.
        /// </summary>
        Exterior = 1,

        /// <summary>
        /// Community amenity.
        /// </summary>
        Community = 2,

        /// <summary>
        /// Utilities amenity.
        /// </summary>
        Utilities = 3,
    }

    /// <summary>
    /// Specifies the status of a booking.
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Pending booking.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Confirmed booking.
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Cancelled booking.
        /// </summary>
        Cancelled = 2,

        /// <summary>
        /// Completed booking.
        /// </summary>
        Completed = 3,
    }
#pragma warning restore SA1402
}
