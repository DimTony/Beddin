using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using FluentAssertions;
using Xunit;

namespace Beddin.UnitTests.Domain.Aggregates.Properties;

public class PropertyTests
{
    private readonly UserId _ownerId = new UserId(Guid.NewGuid());

    [Fact]
    public void Property_ShouldBeCreated_WithValidData()
    {
        // Arrange
        var title = "Beautiful Apartment";
        var description = "A lovely 2-bedroom apartment";
        var primaryImage = "https://example.com/image.jpg";
        var propertyType = PropertyType.Apartment;
        var listingType = ListingType.ForRent;
        var tenor = PropertyTenor.Monthly;
        var price = 2000m;
        var street = "123 Main St";
        var city = "New York";
        var state = "NY";
        var country = "USA";
        var latitude = 40.7128;
        var longitude = -74.0060;
        var bedrooms = 2;
        var bathrooms = 1.5m;
        var squareFeet = 1200m;
        var lotSize = 0m;

        // Act
        var property = Property.Create(
            description,
            _ownerId,
            primaryImage,
            tenor,
            propertyType,
            listingType,
            street,
            city,
            state,
            country,
            latitude,
            longitude,
            bedrooms,
            bathrooms,
            squareFeet,
            lotSize,
            price,
            title
        );

        // Assert
        property.Should().NotBeNull();
        property.Owner.Should().Be(_ownerId);
        property.Title.Should().Be(title);
        property.Description.Should().Be(description);
        property.Type.Should().Be(propertyType);
        property.Listing.Should().Be(listingType);
        property.Tenor.Should().Be(tenor);
        property.Price.Should().Be(price);
        property.Location.Should().NotBeNull();
        property.Latitude.Should().Be(latitude);
        property.Longitude.Should().Be(longitude);
        property.IsPublished.Should().BeFalse();
        property.Status.Should().Be(PropertyStatus.Draft);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Property_ShouldThrowException_WhenTitleIsInvalid(string? invalidTitle)
    {
        // Arrange & Act
        var act = () => Property.Create(
            "Description",
            _ownerId,
            "https://example.com/image.jpg",
            PropertyTenor.Monthly,
            PropertyType.Apartment,
            ListingType.ForRent,
            "123 Main St",
            "New York",
            "NY",
            "USA",
            40.7128,
            -74.0060,
            2,
            1.5m,
            1200m,
            0m,
            2000m,
            invalidTitle
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*title*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Property_ShouldThrowException_WhenPriceIsInvalid(decimal invalidPrice)
    {
        // Arrange & Act
        var act = () => Property.Create(
            "Description",
            _ownerId,
            "https://example.com/image.jpg",
            PropertyTenor.Monthly,
            PropertyType.Apartment,
            ListingType.ForRent,
            "123 Main St",
            "New York",
            "NY",
            "USA",
            40.7128,
            -74.0060,
            2,
            1.5m,
            1200m,
            0m,
            invalidPrice,
            "Title"
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*price*");
    }

    [Fact]
    public void Property_ShouldPublish_Successfully()
    {
        // Arrange
        var property = Property.Create(
            "Description",
            _ownerId,
            "https://example.com/image.jpg",
            PropertyTenor.Monthly,
            PropertyType.Apartment,
            ListingType.ForRent,
            "123 Main St",
            "New York",
            "NY",
            "USA",
            40.7128,
            -74.0060,
            2,
            1.5m,
            1200m,
            0m,
            2000m,
            "Title"
        );

        // Act
        var result = property.Publish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        property.IsPublished.Should().BeTrue();
        property.Status.Should().Be(PropertyStatus.Active);
        property.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public void Property_ShouldUnpublish_Successfully()
    {
        // Arrange
        var property = Property.Create(
            "Description",
            _ownerId,
            "https://example.com/image.jpg",
            PropertyTenor.Monthly,
            PropertyType.Apartment,
            ListingType.ForRent,
            "123 Main St",
            "New York",
            "NY",
            "USA",
            40.7128,
            -74.0060,
            2,
            1.5m,
            1200m,
            0m,
            2000m,
            "Title"
        );
        property.Publish();

        // Act
        var result = property.Unpublish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        property.IsPublished.Should().BeFalse();
        property.Status.Should().Be(PropertyStatus.Draft);
    }

    [Fact]
    public void Property_ShouldIncrementViewCount()
    {
        // Arrange
        var property = Property.Create(
            "Description",
            _ownerId,
            "https://example.com/image.jpg",
            PropertyTenor.Monthly,
            PropertyType.Apartment,
            ListingType.ForRent,
            "123 Main St",
            "New York",
            "NY",
            "USA",
            40.7128,
            -74.0060,
            2,
            1.5m,
            1200m,
            0m,
            2000m,
            "Title"
        );

        // Act
        property.IncrementView();
        property.IncrementView();

        // Assert
        property.ViewCount.Should().Be(2);
    }
}