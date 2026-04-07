// <copyright file="ArchitectureTests.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentAssertions;
using NetArchTest.Rules;

namespace Beddin.ArchitectureTests;

/// <summary>
/// This class contains architecture tests for the Beddin project, ensuring that the different layers of the application (Domain, Application, Infrastructure, API) adhere to the defined architectural rules and constraints. The tests verify that there are no unintended dependencies between layers and that entities in the Domain layer are properly designed to prevent inheritance issues. These tests help maintain a clean and maintainable architecture as the project evolves over time.
/// </summary>
public class ArchitectureTests
{
    private const string DomainNamespace = "Beddin.Domain";
    private const string ApplicationNamespace = "Beddin.Application";
    private const string InfrastructureNamespace = "Beddin.Infrastructure";
    private const string ApiNamespace = "Beddin.API";

    /// <summary>
    /// Tests that the Domain layer does not have any dependencies on the Application layer, ensuring that the Domain layer remains independent and can be reused across different applications without being tightly coupled to the Application layer.
    /// </summary>
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

    /// <summary>
    /// Tests that the Domain layer does not have any dependencies on the Infrastructure layer, ensuring that the Domain layer remains independent and can be reused across different applications without being tightly coupled to the Infrastructure layer.
    /// </summary>
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

    /// <summary>
    /// Tests that the Application layer does not have any dependencies on the Infrastructure layer, ensuring that the Application layer remains independent and can be reused across different applications without being tightly coupled to the Infrastructure layer.
    /// </summary>
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

    /// <summary>
    /// Tests that the Application layer does not have any dependencies on the API layer, ensuring that the Application layer remains independent and can be reused across different applications without being tightly coupled to the API layer.
    /// </summary>
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

    /// <summary>
    /// Tests that all entities in the Domain layer are either sealed or abstract, ensuring that they cannot be inherited from, which helps to prevent issues with inheritance and promotes better encapsulation and maintainability of the domain model.
    /// </summary>
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
}
