using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.RegisterUser;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Domain.Events;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using Xunit.Abstractions;

namespace Beddin.UnitTests.Application.Features.Users.Commands
{
    public class RegisterHandlerTests
    {
        // ── Mocks ────────────────────────────────────────────────────────────────
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly ITestOutputHelper _output;
        private readonly Mock<UserManager<ApplicationUser>> _userManager = new(
           Mock.Of<IUserStore<ApplicationUser>>(),
           null!,  // IOptions<IdentityOptions>
           null!,  // IPasswordHasher<ApplicationUser>
           null!,  // IEnumerable<IUserValidator<ApplicationUser>>
           null!,  // IEnumerable<IPasswordValidator<ApplicationUser>>
           null!,  // ILookupNormalizer
           null!,  // IdentityErrorDescriber
           null!,  // IServiceProvider
           null!); // ILogger<UserManager<ApplicationUser>>

        public RegisterHandlerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private RegisterHandler CreateHandler() =>
            new(_userManager.Object, _userRepo.Object, _unitOfWork.Object);

        private static RegisterCommand ValidCommand(string role = "Buyer") => new(
            FirstName: "Tony",
            LastName: "Stark",
            Email: "tony@beddin.ng",
            Password: "SecureP@ss1",
            Role: role);

        private void LogResult<T>(ApiResponse<T> result)
        {
            _output.WriteLine("=== RESULT DEBUG ===");
            _output.WriteLine($"Success : {result.Success}");
            _output.WriteLine($"Message : {result.Message ?? "(null)"}");

            if (result.Data != null)
            {
                _output.WriteLine($"Data    : {System.Text.Json.JsonSerializer.Serialize(result.Data)}");
            }
            else
            {
                _output.WriteLine("Data    : (null)");
            }

            if (result.Errors != null && result.Errors.Any())
            {
                _output.WriteLine("Errors  :");
                foreach (var error in result.Errors)
                {
                    _output.WriteLine($"  - {error}");
                }
            }
            else
            {
                _output.WriteLine("Errors  : (none)");
            }

            _output.WriteLine("===================");
        }

        // ── Happy path ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Handle_returns_success_with_UserDto_on_valid_command()
        {
            _userRepo
                .Setup(r => r.GetByEmail(ValidCommand("Buyer").Email, default))
                .ReturnsAsync((User?)null);

            _userManager
                .Setup(m => m.FindByEmailAsync(ValidCommand("Buyer").Email))
                .ReturnsAsync((ApplicationUser?)null);

            _userManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), ValidCommand("Buyer").Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManager
                .Setup(m => m.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await CreateHandler().Handle(ValidCommand(), default);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Email.Should().Be("tony@beddin.ng");
            result.Data.Role.Should().Be("Buyer");
        }

        [Fact]
        public async Task Handle_persists_domain_user_and_commits_unit_of_work()
        {
            _userRepo
                .Setup(r => r.GetByEmail(It.IsAny<string>(), default))
                .ReturnsAsync((User?)null);

            _userManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            _userManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManager
                .Setup(m => m.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            await CreateHandler().Handle(ValidCommand(), default);

            _userRepo.Verify(r => r.AddAsync(
                It.Is<User>(u => u.Email == "tony@beddin.ng" && u.Role == UserRole.Buyer),
                default), Times.Once);

            _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Handle_raises_UserCreatedEvent_on_domain_user()
        {
            User? capturedUser = null;

            _userRepo.Setup(r => r.GetByEmail(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(m => m.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>())).ReturnsAsync(IdentityResult.Success);

            _userRepo
                .Setup(r => r.AddAsync(It.IsAny<User>(), default))
                .Callback<User, CancellationToken>((u, _) => capturedUser = u);

            await CreateHandler().Handle(ValidCommand(), default);

            capturedUser!.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserCreatedEvent>();
        }

        // ── Duplicate email guards ────────────────────────────────────────────────

        [Fact]
        public async Task Handle_fails_when_domain_user_already_exists()
        {
            var existing = User.Create("Tony", "Stark", UserRole.Buyer, "tony@beddin.ng");

            _userRepo
                .Setup(r => r.GetByEmail("tony@beddin.ng", default))
                .ReturnsAsync(existing);

            _userManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await CreateHandler().Handle(ValidCommand(), default);
            // LogResult(result);

            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
            result.Errors.Should().Contain("User with this email already exists.");
            result.Message.Should().BeNull();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_fails_when_identity_user_already_exists()
        {
            // Domain user absent but IdentityUser present — catches partial-write leftovers
            _userRepo
                .Setup(r => r.GetByEmail(It.IsAny<string>(), default))
                .ReturnsAsync((User?)null);

            _userManager
                .Setup(m => m.FindByEmailAsync("tony@beddin.ng"))
                .ReturnsAsync(new ApplicationUser { Email = "tony@beddin.ng" });

            var result = await CreateHandler().Handle(ValidCommand(), default);
            //LogResult(result);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("User with this email already exists.");
        }

        // ── Role validation ───────────────────────────────────────────────────────

        [Theory]
        [InlineData("INVALID")]
        [InlineData("")]
        [InlineData("SuperAdmin")]
        public async Task Handle_fails_on_invalid_role(string role)
        {
            _userRepo.Setup(r => r.GetByEmail(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            var result = await CreateHandler().Handle(ValidCommand(role), default);

            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();
            result.Errors.Should().Contain("Invalid role specified.");
            result.Message.Should().BeNull();
            result.Data.Should().BeNull();
        }

        [Theory]
        [InlineData("buyer")]    // lowercase
        [InlineData("BUYER")]    // uppercase
        [InlineData("Buyer")]    // pascal
        public async Task Handle_accepts_role_case_insensitively(string role)
        {
            _userRepo.Setup(r => r.GetByEmail(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(m => m.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>())).ReturnsAsync(IdentityResult.Success);

            var result = await CreateHandler().Handle(ValidCommand(role), default);

            result.Success.Should().BeTrue();
        }

        // ── Identity failure ──────────────────────────────────────────────────────

        [Fact]
        public async Task Handle_fails_and_does_not_persist_domain_user_when_identity_creation_fails()
        {
            _userRepo.Setup(r => r.GetByEmail(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            var identityError = new IdentityError { Description = "Password too weak." };
            _userManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            var result = await CreateHandler().Handle(ValidCommand(), default);
            //LogResult(result);


            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Failed to create user: Password too weak.");

            // Domain user must NOT be saved if identity creation failed
            _userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task Handle_includes_all_identity_errors_in_failure_message()
        {
            _userRepo.Setup(r => r.GetByEmail(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            var errors = new[]
            {
            new IdentityError { Description = "Password too weak." },
            new IdentityError { Description = "Username already taken." }
        };
            _userManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var result = await CreateHandler().Handle(ValidCommand(), default);
            LogResult(result);


            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Failed to create user: Password too weak., Username already taken.");
        }

        // ── Identity user shape ───────────────────────────────────────────────────

        [Fact]
        public async Task Handle_creates_identity_user_with_id_matching_domain_user()
        {
            ApplicationUser? capturedIdentityUser = null;

            _userRepo.Setup(r => r.GetByEmail(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            _userManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, _) => capturedIdentityUser = u)
                .ReturnsAsync(IdentityResult.Success);

            _userManager
                .Setup(m => m.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            User? capturedDomainUser = null;
            _userRepo
                .Setup(r => r.AddAsync(It.IsAny<User>(), default))
                .Callback<User, CancellationToken>((u, _) => capturedDomainUser = u);

            await CreateHandler().Handle(ValidCommand(), default);

            // The IdentityUser.Id must equal the domain User.Id so they stay in sync
            capturedIdentityUser!.Id.Should().Be(capturedDomainUser!.Id.Value.ToString());
            capturedIdentityUser.IsActive.Should().BeFalse();
            capturedIdentityUser.EmailConfirmed.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_deletes_identity_user_when_domain_persistence_fails()
        {
            _userRepo.Setup(r => r.GetByEmail(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(m => m.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>())).ReturnsAsync(IdentityResult.Success);

            // Domain save explodes
            _unitOfWork
                .Setup(u => u.SaveChangesAsync(default))
                .ThrowsAsync(new DbUpdateException("DB unavailable"));

            var act = () => CreateHandler().Handle(ValidCommand(), default);

            await act.Should().ThrowAsync<DbUpdateException>();

            // Compensating delete must have been called
            _userManager.Verify(m => m.DeleteAsync(
                It.Is<ApplicationUser>(u => u.Email == "tony@beddin.ng")),
                Times.Once);
        }
    }
}
