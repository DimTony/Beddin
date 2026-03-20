using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Common
{
    public record UserSessionId(Guid Value)
    {
        public static UserSessionId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    public record UserId(Guid Value)
    {
        public static UserId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
    public record PropertyId(Guid Value)
    {
        public static PropertyId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    public record PropertyImageId(Guid Value)
    {
        public static PropertyImageId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    public record AmenityId(Guid Value)
    {
        public static AmenityId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    public record PropertyAmenityId(Guid Value)
    {
        public static PropertyAmenityId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    public record FavoriteId(Guid Value)
    {
        public static FavoriteId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    public record BookingId(Guid Value)
    {
        public static BookingId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

}
