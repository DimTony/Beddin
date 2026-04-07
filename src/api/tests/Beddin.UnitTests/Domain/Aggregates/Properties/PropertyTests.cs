// <copyright file="PropertyTests.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using FluentAssertions;

namespace Beddin.UnitTests.Domain.Aggregates.Properties;

/// <summary>
/// Contains unit tests for the Property domain model, verifying correct behavior for property creation, validation,
/// publishing, unpublishing, and view count tracking. These tests ensure that the Property class enforces input
/// constraints and maintains expected state transitions.
/// </summary>
/// <remarks>This class uses xUnit attributes to define test cases for various scenarios, including successful
/// creation, validation of required fields, and state changes such as publishing and unpublishing. The tests help
/// maintain the integrity of the Property API by ensuring that invalid data is rejected and that business rules are
/// enforced.</remarks>
public class PropertyTests
{
    private readonly UserId ownerId = new UserId(Guid.NewGuid());

    /// <summary>
    /// Tests that a property can be successfully created with valid data, ensuring that all required fields are set correctly and that the initial state of the property is as expected (e.g., not published, draft status). This test verifies the integrity of the property creation logic and validates that the constructor and factory method enforce necessary constraints on the input data.
    /// </summary>
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
            this.ownerId,
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
            title);

        // Assert
        property.Should().NotBeNull();
        property.Owner.Should().Be(this.ownerId);
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

    /// <summary>
    /// Tests that creating a property with an invalid title (empty, whitespace, or null) throws an ArgumentException, ensuring that the property creation logic enforces the requirement for a valid title. This test validates that the constructor or factory method correctly identifies and handles invalid input for the title field, maintaining data integrity and preventing the creation of properties with invalid titles.
    /// </summary>
    /// <param name="invalidTitle">The invalid title to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Property_ShouldThrowException_WhenTitleIsInvalid(string? invalidTitle)
    {
        // Arrange & Act
        var act = () => Property.Create(
            "Description",
            this.ownerId,
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
            invalidTitle);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*title*");
    }

    /// <summary>
    /// Tests that creating a property with an invalid price (zero, or negative value) throws an ArgumentException, ensuring that the property creation logic enforces the requirement for a valid price. This test validates that the constructor or factory method correctly identifies and handles invalid input for the price field, maintaining data integrity and preventing the creation of properties with invalid prices.
    /// </summary>
    /// <param name="invalidPrice">The invalid price to test.</param>
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Property_ShouldThrowException_WhenPriceIsInvalid(decimal invalidPrice)
    {
        // Arrange & Act
        var act = () => Property.Create(
            "Description",
            this.ownerId,
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
            "Title");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*price*");
    }

    /// <summary>
    /// Tests that a property can be published if satisfactorily created.
    /// </summary>
    [Fact]
    public void Property_ShouldPublish_Successfully()
    {
        // Arrange
        var property = Property.Create(
            "Description",
            this.ownerId,
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
            "Title");

        // Act
        var result = property.Publish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        property.IsPublished.Should().BeTrue();
        property.Status.Should().Be(PropertyStatus.Active);
        property.PublishedAt.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that a property can be unpublished if need be.
    /// </summary>
    [Fact]
    public void Property_ShouldUnpublish_Successfully()
    {
        // Arrange
        var property = Property.Create(
            "Description",
            this.ownerId,
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
            "Title");
        property.Publish();

        // Act
        var result = property.Unpublish();

        // Assert
        result.IsSuccess.Should().BeTrue();
        property.IsPublished.Should().BeFalse();
        property.Status.Should().Be(PropertyStatus.Draft);
    }

    /// <summary>
    /// Verifies that calling the IncrementView method on a Property instance correctly increments its ViewCount
    /// property.
    /// </summary>
    /// <remarks>This test ensures that each call to IncrementView increases the ViewCount by one, confirming
    /// the expected behavior for tracking property views.</remarks>
    [Fact]
    public void Property_ShouldIncrementViewCount()
    {
        // Arrange
        var property = Property.Create(
            "Description",
            this.ownerId,
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
            "Title");

        // Act
        property.IncrementView();
        property.IncrementView();

        // Assert
        property.ViewCount.Should().Be(2);
    }
}
