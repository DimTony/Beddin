using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Beddin.UnitTests.Domain.Aggregates.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUser_WithValidData()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var role = UserRoles.Buyer;

        // Act
        var user = User.Create(firstName, lastName, role, email);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeNull();
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email);
        user.Role.Should().Be(role);
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent_WhenUserIsCreated()
    {
        // Arrange & Act
        var user = User.Create("John", "Doe", UserRoles.Buyer, "john@example.com");

        // Assert
        user.DomainEvents.Should().HaveCount(1);
        user.DomainEvents.First().Should().BeOfType<UserCreatedEvent>();
        
        var domainEvent = user.DomainEvents.First() as UserCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(user.Id);
        domainEvent.Email.Should().Be("john@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowException_WhenEmailIsInvalid(string? invalidEmail)
    {
        // Arrange & Act
        var act = () => User.Create("John", "Doe", UserRoles.Buyer, invalidEmail);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("email");
    }

    [Fact]
    public void UpdateEmail_ShouldUpdateEmail_AndRaiseDomainEvent()
    {
        // Arrange
        var user = User.Create("John", "Doe", UserRoles.Buyer, "old@example.com");
        user.ClearDomainEvents(); // Clear creation event
        var newEmail = "new@example.com";

        // Act
        user.UpdateEmail(newEmail);

        // Assert
        user.Email.Should().Be(newEmail);
        user.UpdatedAt.Should().BeAfter(user.CreatedAt);
        user.DomainEvents.Should().HaveCount(1);
        user.DomainEvents.First().Should().BeOfType<EmailUpdatedEvent>();
    }

    [Fact]
    public void UpdateRole_ShouldUpdateRole_AndRaiseDomainEvent()
    {
        // Arrange
        var user = User.Create("John", "Doe", UserRoles.Buyer, "john@example.com");
        user.ClearDomainEvents();
        var newRole = UserRoles.Owner;

        // Act
        user.UpdateRole(newRole);

        // Assert
        user.Role.Should().Be(newRole);
        user.UpdatedAt.Should().BeAfter(user.CreatedAt);
        user.DomainEvents.Should().HaveCount(1);
        user.DomainEvents.First().Should().BeOfType<RoleUpdatedEvent>();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse_AndRaiseDomainEvent()
    {
        // Arrange
        var user = User.Create("John", "Doe", UserRoles.Buyer, "john@example.com");
        user.ClearDomainEvents();

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().BeAfter(user.CreatedAt);
        user.DomainEvents.Should().HaveCount(1);
        user.DomainEvents.First().Should().BeOfType<UserDeactivatedEvent>();
    }
}
