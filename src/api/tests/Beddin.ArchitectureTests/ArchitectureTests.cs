using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Beddin.ArchitectureTests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Beddin.Domain";
    private const string ApplicationNamespace = "Beddin.Application";
    private const string InfrastructureNamespace = "Beddin.Infrastructure";
    private const string ApiNamespace = "Beddin.API";

    [Fact]
    public void Domain_ShouldNotHaveDependencyOn_Application()
    {
        // Arrange
        var assembly = typeof(Beddin.Domain.Common.Entity<>).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on Application layer");
    }

    [Fact]
    public void Domain_ShouldNotHaveDependencyOn_Infrastructure()
    {
        // Arrange
        var assembly = typeof(Beddin.Domain.Common.Entity<>).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on Infrastructure layer");
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOn_Infrastructure()
    {
        // Arrange
        var assembly = typeof(Beddin.Application.Common.Interfaces.IRepository<,>).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application layer should not depend on Infrastructure layer");
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOn_API()
    {
        // Arrange
        var assembly = typeof(Beddin.Application.Common.Interfaces.IRepository<,>).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application layer should not depend on API layer");
    }

    [Fact]
    public void Entities_ShouldBeSealed_OrAbstract()
    {
        // Arrange
        var assembly = typeof(Beddin.Domain.Common.Entity<>).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Beddin.Domain.Common.Entity<>))
            .Should()
            .BeSealed()
            .Or()
            .BeAbstract()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All entities should be sealed or abstract to prevent inheritance issues");
    }

    [Fact]
    public void ValueObjects_ShouldBeSealed()
    {
        // Arrange
        var assembly = typeof(Beddin.Domain.Aggregates.Users.UserRoles).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Beddin.Domain.Aggregates.Users.UserRoles))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All value objects should be sealed");
    }

    //[Fact]
    //public void ValueObjects_ShouldBeSealed()
    //{
    //    // Arrange
    //    var assembly = typeof(Beddin.Domain.Common.ValueObject).Assembly;

    //    // Act
    //    var result = Types.InAssembly(assembly)
    //        .That()
    //        .Inherit(typeof(Beddin.Domain.Common.ValueObject))
    //        .Should()
    //        .BeSealed()
    //        .GetResult();

    //    // Assert
    //    result.IsSuccessful.Should().BeTrue(
    //        "All value objects should be sealed");
    //}
}