// <copyright file="Property.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Domain.Events;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Beddin.Domain.Aggregates.Properties
{
    /// <summary>
    /// Represents a property listing in the system.
    /// </summary>
    public sealed class Property : AggregateRoot<PropertyId>
    {
        private readonly List<PropertyImage> images = new();
        private readonly List<PropertyAmenity> amenities = new();
        private readonly List<Favorite> favorites = new();
        private readonly List<Booking> bookings = new();
        private readonly List<Inquiry> inquiries = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        private Property()
        {
        }

        /// <summary>
        /// Gets the title of the property.
        /// </summary>
        public string Title { get; private set; } = default!;

        /// <summary>
        /// Gets the description of the property.
        /// </summary>
        public string Description { get; private set; } = default!;

        /// <summary>
        /// Gets the owner identifier of the property.
        /// </summary>
        public UserId Owner { get; private set; } = null!;

        /// <summary>
        /// Gets the primary image URL of the property.
        /// </summary>
        public string PrimaryImage { get; private set; } = default!;

        /// <summary>
        /// Gets the tenor of the property.
        /// </summary>
        public PropertyTenor Tenor { get; private set; } = PropertyTenor.Annually;

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public PropertyType Type { get; private set; }

        /// <summary>
        /// Gets the status of the property.
        /// </summary>
        public PropertyStatus Status { get; private set; }

        /// <summary>
        /// Gets the listing type of the property.
        /// </summary>
        public ListingType Listing { get; private set; }

        /// <summary>
        /// Gets the street address of the property.
        /// </summary>
        public string Street { get; private set; } = default!;

        /// <summary>
        /// Gets the city of the property.
        /// </summary>
        public string City { get; private set; } = default!;

        /// <summary>
        /// Gets the state of the property.
        /// </summary>
        public string State { get; private set; } = default!;

        /// <summary>
        /// Gets the country of the property.
        /// </summary>
        public string Country { get; private set; } = default!;

        /// <summary>
        /// Gets the latitude coordinate of the property.
        /// </summary>
        public double Latitude { get; private set; }

        /// <summary>
        /// Gets the longitude coordinate of the property.
        /// </summary>
        public double Longitude { get; private set; }

        /// <summary>
        /// Gets the geographic location point of the property.
        /// </summary>
        public Point? Location { get; private set; }

        /// <summary>
        /// Gets the number of bedrooms in the property.
        /// </summary>
        public int Bedrooms { get; private set; }

        /// <summary>
        /// Gets the number of bathrooms in the property.
        /// </summary>
        public decimal Bathrooms { get; private set; }

        /// <summary>
        /// Gets the square footage of the property.
        /// </summary>
        public decimal SquareFeet { get; private set; }

        /// <summary>
        /// Gets the lot size of the property.
        /// </summary>
        public decimal LotSize { get; private set; }

        /// <summary>
        /// Gets the year the property was built.
        /// </summary>
        public DateOnly? YearBuilt { get; private set; }

        /// <summary>
        /// Gets the price of the property.
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the property is published.
        /// </summary>
        public bool IsPublished { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the property is featured.
        /// </summary>
        public bool IsFeatured { get; private set; }

        /// <summary>
        /// Gets the number of views for the property.
        /// </summary>
        public int ViewCount { get; private set; }

        /// <summary>
        /// Gets the number of favorites for the property.
        /// </summary>
        public int FavoriteCount { get; private set; }

        /// <summary>
        /// Gets the date and time when the property was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the property was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the property was published.
        /// </summary>
        public DateTime? PublishedAt { get; private set; }

        /// <summary>
        /// Gets the collection of images for the property.
        /// </summary>
        public IReadOnlyCollection<PropertyImage> Images => this.images;

        /// <summary>
        /// Gets the collection of amenities for the property.
        /// </summary>
        public IReadOnlyCollection<PropertyAmenity> Amenities => this.amenities;

        /// <summary>
        /// Gets the collection of favorites for the property.
        /// </summary>
        public IReadOnlyCollection<Favorite> Favorites => this.favorites;

        /// <summary>
        /// Gets the collection of bookings for the property.
        /// </summary>
        public IReadOnlyCollection<Booking> Bookings => this.bookings;

        /// <summary>
        /// Gets the collection of inquiries for the property.
        /// </summary>
        public IReadOnlyCollection<Inquiry> Inquiries => this.inquiries;

        /// <summary>
        /// Creates a new <see cref="Property"/> instance.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="owner">The owner identifier.</param>
        /// <param name="primaryImage">The primary image URL.</param>
        /// <param name="tenor">The tenor.</param>
        /// <param name="type">The property type.</param>
        /// <param name="listing">The listing type.</param>
        /// <param name="street">The street address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="country">The country.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="bedrooms">The number of bedrooms.</param>
        /// <param name="bathrooms">The number of bathrooms.</param>
        /// <param name="squareFeet">The square footage.</param>
        /// <param name="lotSize">The lot size.</param>
        /// <param name="price">The price.</param>
        /// <param name="title">The title.</param>
        /// <param name="yearBuilt">The year built.</param>
        /// <returns>A new <see cref="Property"/> instance.</returns>
        public static Property Create(
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
            string? title,
            DateOnly? yearBuilt = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title is required");
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero");
            }

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
                FavoriteCount = 0,
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

        /// <summary>
        /// Publishes the property.
        /// </summary>
        /// <returns>A result indicating success or failure.</returns>
        public Result Publish()
        {
            if (this.Owner is null)
            {
                return Result.Failure("Property requires an owner to be published");
            }

            if (this.IsPublished)
            {
                return Result.Success();
            }

            this.IsPublished = true;
            this.Status = PropertyStatus.Active;
            this.PublishedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new PropertyPublishedEvent(this.Id, this.Owner));

            return Result.Success();
        }

        /// <summary>
        /// Unpublishes the property.
        /// </summary>
        /// <returns>A result indicating success or failure.</returns>
        public Result Unpublish()
        {
            if (this.Owner is null)
            {
                return Result.Failure("Property requires an owner to be unpublished");
            }

            this.IsPublished = false;
            this.Status = PropertyStatus.Draft;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new PropertyUnpublishedEvent(this.Id, this.Owner));

            return Result.Success();
        }

        /// <summary>
        /// Increments the view count for the property.
        /// </summary>
        public void IncrementView()
        {
            this.ViewCount++;
        }

        /// <summary>
        /// Adds a favorite to the property.
        /// </summary>
        public void AddFavorite()
        {
            this.FavoriteCount++;
        }

        /// <summary>
        /// Removes a favorite from the property.
        /// </summary>
        public void RemoveFavorite()
        {
            if (this.FavoriteCount > 0)
            {
                this.FavoriteCount--;
            }
        }

        /// <summary>
        /// Updates the price of the property.
        /// </summary>
        /// <param name="newPrice">The new price.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result UpdatePrice(decimal newPrice)
        {
            if (this.Owner is null)
            {
                return Result.Failure("Property requires an owner to update price");
            }

            if (newPrice <= 0)
            {
                throw new ArgumentException("Invalid price");
            }

            this.Price = newPrice;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new PriceUpdatedEvent(this.Id, this.Owner, newPrice));

            return Result.Success();
        }
    }
}
