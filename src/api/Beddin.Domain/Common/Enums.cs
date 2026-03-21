using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Common
{
    //public enum UserRole
    //{
    //    Buyer = 0,
    //    Owner = 1,
    //    Admin = 2
    //}

    public enum InquiryStatus
    {
        Unread = 0,
        Read = 1,
        Replied = 2,
        Archived = 3
    }

    public enum InquiryType
    {
        General = 0,
        ViewingRequest = 1,
        PriceNegotiation = 2
    }
    public enum TransactionType
    {
        Sale = 0,
        Rent = 1,
        ShortLet = 2,
        JointVenture = 3
    }
    public enum AlertFrequency
    {
        Immediate = 0,
        Daily = 1,
        Weekly = 2
    }

    public enum PropertyType
    {
        House = 0,
        Apartment = 1,
        Condo = 2,
        Townhouse = 3,
        Land = 4,
        Commercial = 5
    }

    public enum PropertyTenor
    {
        Daily = 0,
        Weekly = 1,
        BiWeekly = 2,
        Monthly = 3,
        Annually = 4,
        BiAnnually = 5,
        Outright = 6
    }

    public enum ListingType
    {
        ForSale = 0,
        ForRent = 1
    }

    public enum PropertyStatus
    {
        Draft = 0,
        Active = 1,
        Pending = 2,
        Sold = 3,
        Inactive = 4,
        Withdrawn = 5,
        Expired = 6,
        Deleted = 7
    }

    public enum AmenityCategory
    {
        Interior = 0,
        Exterior = 1,
        Community = 2,
        Utilities = 3
    }

    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }
}
