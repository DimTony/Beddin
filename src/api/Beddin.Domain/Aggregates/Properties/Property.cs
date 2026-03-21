using NetTopologySuite;
using NetTopologySuite.Geometries;
using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.Properties
{
    public class Property : AggregateRoot<PropertyId>
    {
        public string Title { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public UserId Owner { get; private set; } = null!;

        public string PrimaryImage { get; private set; } = default!;
        public PropertyTenor Tenor { get; private set; } = PropertyTenor.Annually;
        public PropertyType Type { get; private set; }
        public PropertyStatus Status { get; private set; }
        public ListingType Listing { get; private set; }

        public string Street { get; private set; } = default!;
        public string City { get; private set; } = default!;
        public string State { get; private set; } = default!;
        public string Country { get; private set; } = default!;

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public Point? Location { get; private set; }

        public int Bedrooms { get; private set; }
        public decimal Bathrooms { get; private set; }
        public decimal SquareFeet { get; private set; }
        public decimal LotSize { get; private set; }
        public DateOnly? YearBuilt { get; private set; }

        public decimal Price { get; private set; }

        public bool IsPublished { get; private set; }
        public bool IsFeatured { get; private set; }

        public int ViewCount { get; private set; }
        public int FavoriteCount { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }

        private readonly List<PropertyImage> _images = new();
        public IReadOnlyCollection<PropertyImage> Images => _images;

        private readonly List<PropertyAmenity> _amenities = new();
        public IReadOnlyCollection<PropertyAmenity> Amenities => _amenities;

        private readonly List<Favorite> _favorites = new();
        public IReadOnlyCollection<Favorite> Favorites => _favorites;

        private readonly List<Booking> _bookings = new();
        public IReadOnlyCollection<Booking> Bookings => _bookings;

        private Property() { }

        public static Property Create(
            string title,
            string description,
            UserId owner,
            string primaryImage,
            PropertyTenor tenor,
            PropertyType type,
            ListingType listing,
            string street,
            string city,
            string state,
            string country,
            double latitude,
            double longitude,
            int bedrooms,
            decimal bathrooms,
            decimal squareFeet,
            decimal lotSize,
            decimal price,
            DateOnly? yearBuilt = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero");

            var property = new Property
            {
                Id = PropertyId.New(),
                Title = title,
                Description = description,
                Owner = owner,
                PrimaryImage = primaryImage,
                Tenor = tenor,
                Type = type,
                Listing = listing,
                Status = PropertyStatus.Draft,

                Street = street,
                City = city,
                State = state,
                Country = country,

                Latitude = latitude,
                Longitude = longitude,

                Bedrooms = bedrooms,
                Bathrooms = bathrooms,
                SquareFeet = squareFeet,
                LotSize = lotSize,
                Price = price,
                YearBuilt = yearBuilt,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPublished = false,
                IsFeatured = false,
                ViewCount = 0,
                FavoriteCount = 0
            };

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            property.Location = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

            property.RaiseDomainEvent(new PropertyCreatedEvent(
                property.Id,
                property.Title,
                property.Price,
                property.Owner));

            return property;
        }

        public Result Publish()
        {
            if (Owner is null)
                return Result.Failure("Property requires an owner to be published");

            if (IsPublished)
                return Result.Success();

            IsPublished = true;
            Status = PropertyStatus.Active;
            PublishedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PropertyPublishedEvent(Id, Owner));

            return Result.Success();
        }

        public Result Unpublish()
        {
            if (Owner is null)
                return Result.Failure("Property requires an owner to be unpublished");

            IsPublished = false;
            Status = PropertyStatus.Draft;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PropertyUnpublishedEvent(Id, Owner));

            return Result.Success();
        }

        public void IncrementView()
        {
            ViewCount++;
        }

        public void AddFavorite()
        {
            FavoriteCount++;
        }

        public void RemoveFavorite()
        {
            if (FavoriteCount > 0)
                FavoriteCount--;
        }

        public Result UpdatePrice(decimal newPrice)
        {
            if (Owner is null)
                return Result.Failure("Property requires an owner to update price");

            if (newPrice <= 0)
                throw new ArgumentException("Invalid price");

            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PriceUpdatedEvent(Id, Owner, newPrice));

            return Result.Success();
        }

        public Result AddAmenity(AmenityId amenityId)
        {
            if (Owner is null)
                return Result.Failure("Property requires an owner to add amenity");

            if (_amenities.Any(a => a.AmenityId == amenityId))
                return Result.Success();

            _amenities.Add(PropertyAmenity.Create(Id, amenityId));

            RaiseDomainEvent(new AmenityAddedEvent(Id, Owner, amenityId));


            return Result.Success();
        }

        public Result RemoveAmenity(AmenityId amenityId)
        {
            if (Owner is null)
                return Result.Failure("Property requires an owner to remove amenity");

            var amenity = _amenities.FirstOrDefault(a => a.AmenityId == amenityId);

            if (amenity is null)
                return Result.Success();

            _amenities.Remove(amenity);

            RaiseDomainEvent(new AmenityRemovedEvent(Id, Owner, amenityId));


            return Result.Success();
        }

        public Result SetAmenities(IEnumerable<AmenityId> amenityIds)
        {
            if (Owner is null)
                return Result.Failure("Property requires an owner to set amenities");

            _amenities.Clear();

            foreach (var id in amenityIds.Distinct())
            {
                _amenities.Add(PropertyAmenity.Create(Id, id));
            }

            RaiseDomainEvent(new AmenitiesSetEvent(Id, Owner, amenityIds));


            return Result.Success();

        }
    }
}
