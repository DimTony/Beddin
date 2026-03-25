//using Beddin.Application.Common.DTOs;
//using Beddin.Application.Common.Options;
//using Beddin.Application.Features.Users.Commands.RegisterUser;
//using Beddin.Domain.Aggregates.Users;
//using Beddin.Domain.Common;
//using Beddin.Infrastructure.Persistence;
//using FluentAssertions;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using System.Net;
//using System.Net.Http.Json;
//using System.Text.Json;
//using Xunit;
//using Xunit.Abstractions;

//namespace Beddin.IntegrationTests.API;

//public class RegisterEndpointTests : ApiIntegrationTestBase
//{
//    private readonly ITestOutputHelper _output;

//    public RegisterEndpointTests(BeddinApiFactory factory, ITestOutputHelper output)
//        : base(factory)
//    {
//        _output = output;
//    }

//    private static UserRegistrationPayload ValidPayload(string email = "test@beddin.ng") => new(
//        FirstName: "Tony",
//        LastName: "Stark",
//        Email: email,
//        Password: "SecureP@ss123!",
//        Role: UserRoles.Buyer
//    );

//    [Fact]
//    public async Task POST_Register_WithValidData_Returns200WithUserDto()
//    {
//        var payload = ValidPayload("valid.user@beddin.ng");

//        var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//        response.StatusCode.Should().Be(HttpStatusCode.OK);

//        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//        body.Should().NotBeNull();
//        body!.Success.Should().BeTrue();
//        body.Data.Should().NotBeNull();
//        body.Data!.Email.Should().Be(payload.Email);
//        body.Data.FirstName.Should().Be(payload.FirstName);
//        body.Data.LastName.Should().Be(payload.LastName);
//        body.Data.Role.Should().Be(payload.Role);
//        body.Data.IsActive.Should().BeTrue();
//    }

//    //[Fact]
//    //public async Task POST_Register_WithValidData_PersistsUserToDatabase()
//    //{
//    //    var payload = ValidPayload("persisted@beddin.ng");

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.OK);

//    //    using var scope = Factory.Services.CreateScope();
//    //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    //    var user = await db.AppUsers
//    //        .FirstOrDefaultAsync(u => u.Email == payload.Email);

//    //    user.Should().NotBeNull();
//    //    user!.FirstName.Should().Be(payload.FirstName);
//    //    user.LastName.Should().Be(payload.LastName);
//    //    user.Email.Should().Be(payload.Email);
//    //    Enum.TryParse<UserRole>(payload.Role, out var expectedRole).Should().BeTrue();
//    //    user.Role.Should().Be(expectedRole);
//    //    user.IsActive.Should().BeTrue();
//    //}

//    //[Fact]
//    //public async Task POST_Register_WithDuplicateEmail_Returns400()
//    //{
//    //    var email = "duplicate@beddin.ng";
//    //    var firstPayload = ValidPayload(email);
//    //    var secondPayload = ValidPayload(email) with { FirstName = "Different" };

//    //    await Client.PostAsJsonAsync("/Authentication/Register", firstPayload);
//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", secondPayload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

//    //    var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//    //    body.Should().NotBeNull();
//    //    body!.Success.Should().BeFalse();
//    //    body.Errors.Should().NotBeEmpty();
//    //}

//    //[Theory]
//    //[InlineData("", "Doe", "valid@beddin.ng", "SecureP@ss123!", UserRoles.Buyer)]
//    //[InlineData("John", "", "valid@beddin.ng", "SecureP@ss123!", UserRoles.Buyer)]
//    //[InlineData("John", "Doe", "", "SecureP@ss123!", UserRoles.Buyer)]
//    //[InlineData("John", "Doe", "invalid-email", "SecureP@ss123!", UserRoles.Buyer)]
//    //[InlineData("John", "Doe", "valid@beddin.ng", "", UserRoles.Buyer)]
//    //[InlineData("John", "Doe", "valid@beddin.ng", "weak", UserRoles.Buyer)]
//    //public async Task POST_Register_WithInvalidData_Returns400(
//    //    string firstName, string lastName, string email, string password, string role)
//    //{
//    //    var payload = new UserRegistrationPayload(firstName, lastName, email, password, role);

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//    //}

//    //[Fact]
//    //public async Task POST_Register_WithInvalidRole_Returns400()
//    //{
//    //    var payload = ValidPayload("invalid.role@beddin.ng") with { Role = "SuperAdmin" };

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

//    //    var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//    //    body.Should().NotBeNull();
//    //    body!.Success.Should().BeFalse();
//    //    body.Errors.Should().NotBeEmpty();
//    //}

//    //[Theory]
//    //[InlineData(UserRoles.Buyer)]
//    //[InlineData(UserRoles.Owner)]
//    //[InlineData(UserRoles.Admin)]
//    //public async Task POST_Register_WithAllValidRoles_Returns200(string role)
//    //{
//    //    var payload = ValidPayload($"{role.ToLower()}.test@beddin.ng") with { Role = role };

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.OK);

//    //    var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//    //    body.Should().NotBeNull();
//    //    body!.Success.Should().BeTrue();
//    //    body.Data!.Role.Should().Be(role);
//    //}

//    //[Fact]
//    //public async Task POST_Register_SetsTimestampsCorrectly()
//    //{
//    //    var beforeRegistration = DateTime.UtcNow;
//    //    var payload = ValidPayload("timestamp.test@beddin.ng");

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);
//    //    var afterRegistration = DateTime.UtcNow;

//    //    response.StatusCode.Should().Be(HttpStatusCode.OK);

//    //    using var scope = Factory.Services.CreateScope();
//    //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    //    var user = await db.AppUsers
//    //        .FirstOrDefaultAsync(u => u.Email == payload.Email);

//    //    user.Should().NotBeNull();
//    //    user!.CreatedAt.Should().BeOnOrAfter(beforeRegistration)
//    //        .And.BeOnOrBefore(afterRegistration);
//    //    user.UpdatedAt.Should().BeCloseTo(user.CreatedAt, precision: TimeSpan.FromSeconds(1));
//    //}

//    //[Fact]
//    //public async Task POST_Register_ReturnsUserDtoWithCorrectTimestamp()
//    //{
//    //    var beforeRegistration = DateTime.UtcNow;
//    //    var payload = ValidPayload("dto.timestamp@beddin.ng");

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);
//    //    var afterRegistration = DateTime.UtcNow;

//    //    response.StatusCode.Should().Be(HttpStatusCode.OK);

//    //    var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//    //    body.Should().NotBeNull();
//    //    body!.Data.Should().NotBeNull();
//    //    body.Data!.CreatedAt.Should().BeOnOrAfter(beforeRegistration)
//    //        .And.BeOnOrBefore(afterRegistration);
//    //}

//    //[Fact]
//    //public async Task POST_Register_WithMultipleUsers_CreatesUniqueIds()
//    //{
//    //    var payload1 = ValidPayload("user1@beddin.ng");
//    //    var payload2 = ValidPayload("user2@beddin.ng");

//    //    var response1 = await Client.PostAsJsonAsync("/Authentication/Register", payload1);
//    //    var response2 = await Client.PostAsJsonAsync("/Authentication/Register", payload2);

//    //    response1.StatusCode.Should().Be(HttpStatusCode.OK);
//    //    response2.StatusCode.Should().Be(HttpStatusCode.OK);

//    //    var body1 = await response1.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//    //    var body2 = await response2.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

//    //    body1!.Data!.Id.Should().NotBe(body2!.Data!.Id);
//    //}

//    //[Fact]
//    //public async Task POST_Register_ReturnsApiResponseWithSuccessFlag()
//    //{
//    //    var payload = ValidPayload("success.flag@beddin.ng");

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.OK);

//    //    var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//    //    body.Should().NotBeNull();
//    //    body!.Success.Should().BeTrue();
//    //    body.Timestamp.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromMinutes(1));
//    //}

//    //[Fact]
//    //public async Task POST_Register_WithFailure_ReturnsApiResponseWithErrors()
//    //{
//    //    var payload = ValidPayload("failure@beddin.ng") with { Role = "InvalidRole" };

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

//    //    var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
//    //    body.Should().NotBeNull();
//    //    body!.Success.Should().BeFalse();
//    //    body.Data.Should().BeNull();
//    //    body.Errors.Should().NotBeEmpty();
//    //}

//    //[Fact]
//    //public async Task POST_Register_DoesNotReturnSensitiveInformation()
//    //{
//    //    var payload = ValidPayload("security.test@beddin.ng");

//    //    var response = await Client.PostAsJsonAsync("/Authentication/Register", payload);

//    //    response.StatusCode.Should().Be(HttpStatusCode.OK);

//    //    var responseContent = await response.Content.ReadAsStringAsync();
//    //    responseContent.Should().NotContain(payload.Password);
//    //    responseContent.Should().NotContain("PasswordHash");
//    //}

//    private async Task LogResponse(HttpResponseMessage response)
//    {
//        var body = await response.Content.ReadAsStringAsync();
//        _output.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
//        _output.WriteLine($"Body: {body}");
//    }
//}