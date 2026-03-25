//using Beddin.Application.Common.DTOs;
//using Beddin.Domain.Aggregates.Users;
//using Beddin.Domain.Common;
//using Beddin.Infrastructure.Persistence;
//using FluentAssertions;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.Extensions.Logging.Abstractions;
//using Xunit.Abstractions;

//namespace Beddin.IntegrationTests.Infrastructure.Repositories
//{
//    public class ApplicationUserStoreTests : IntegrationTestBase
//    {
//        private const string Identity = "000000-0000-0000-00000-0000";
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly ITestOutputHelper _output;

//        public ApplicationUserStoreTests(ITestOutputHelper output)
//        {
//            _output = output;
//            _userManager = BuildUserManager(DbContext);
//        }

//        private static UserManager<ApplicationUser> BuildUserManager(AppDbContext db)
//        {
//            var store = new UserStore<ApplicationUser>(db);

//            return new UserManager<ApplicationUser>(
//                store,
//                null!,
//                new PasswordHasher<ApplicationUser>(),
//                new IUserValidator<ApplicationUser>[] { new UserValidator<ApplicationUser>() },
//                new IPasswordValidator<ApplicationUser>[] { new PasswordValidator<ApplicationUser>() },
//                new UpperInvariantLookupNormalizer(),
//                new IdentityErrorDescriber(),
//                null!,
//                NullLogger<UserManager<ApplicationUser>>.Instance);
//        }
//        private void LogResult(IdentityResult result)
//        {
//            _output.WriteLine("=== IDENTITY RESULT DEBUG ===");

//            if (result.Succeeded)
//            {
//                _output.WriteLine("Result: Succeede");
//            }
//            else
//            {
//                _output.WriteLine("Result: Failed");
//                foreach (var error in result.Errors)
//                {
//                    _output.WriteLine($"- Code: {error.Code}, Description: {error.Description}");
//                }
//            }

//            _output.WriteLine("============================");
//        }

//        // ── Tests ─────────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task CreateAsync_persists_identity_user_with_hashed_password()
//        {
//            var identityUser = new ApplicationUser
//            {
//                Id = Guid.NewGuid().ToString(),
//                UserName = "hash@beddin.ng",
//                Email = "hash@beddin.ng",
//                EmailConfirmed = false,
//                //IsActive = false,
//                FailedLoginAttempts = 0
//            };

//            var result = await _userManager.CreateAsync(identityUser, "SecureP@ss1");
//            //LogResult(result);

//            result.Succeeded.Should().BeTrue();

//            var loaded = await _userManager.FindByEmailAsync("hash@beddin.ng");
//            loaded.Should().NotBeNull();
//            loaded!.PasswordHash.Should().NotBe("SecureP@ss1");
//            loaded.PasswordHash.Should().NotBeNullOrEmpty();
//        }

//        [Fact]
//        public async Task CreateAsync_stores_identity_user_id_matching_domain_user_id()
//        {
//            var domainUser = User.Create("Tony", "Stark", UserRole.Buyer, "sync@beddin.ng");

//            var identityUser = new ApplicationUser
//            {
//                Id = domainUser.Id.Value.ToString(),
//                UserName = "sync@beddin.ng",
//                Email = "sync@beddin.ng",
//                EmailConfirmed = false,
//                IsActive = false,
//                FailedLoginAttempts = 0
//            };

//            var result = await _userManager.CreateAsync(identityUser, "SecureP@ss1");
//            LogResult(result);

//            result.Succeeded.Should().BeTrue();

//            var loaded = await _userManager.FindByEmailAsync("sync@beddin.ng");
//            loaded!.Id.Should().Be(domainUser.Id.Value.ToString());
//        }

//        [Fact]
//        public async Task AddClaimAsync_persists_role_claim_correctly()
//        {
//            var identityUser = new ApplicationUser
//            {
//                Id = Guid.NewGuid().ToString(),
//                UserName = "claim@beddin.ng",
//                Email = "claim@beddin.ng",
//                EmailConfirmed = false,
//                IsActive = false,
//                FailedLoginAttempts = 0
//            };

//            await _userManager.CreateAsync(identityUser, "SecureP@ss1");
//            await _userManager.AddClaimAsync(identityUser,
//                new System.Security.Claims.Claim("role", "Buyer"));

//            var claims = await _userManager.GetClaimsAsync(identityUser);
//            claims.Should().ContainSingle(c => c.Type == "role" && c.Value == "Buyer");
//        }

//        [Fact]
//        public async Task DeleteAsync_removes_identity_user_and_validates_compensation_works()
//        {
//            var identityUser = new ApplicationUser
//            {
//                Id = Guid.NewGuid().ToString(),
//                UserName = "delete@beddin.ng",
//                Email = "delete@beddin.ng",
//                EmailConfirmed = false,
//                IsActive = false,
//                FailedLoginAttempts = 0
//            };

//            await _userManager.CreateAsync(identityUser, "SecureP@ss1");
//            await _userManager.DeleteAsync(identityUser);

//            var loaded = await _userManager.FindByEmailAsync("delete@beddin.ng");
//            loaded.Should().BeNull();
//        }
//    }
//    //public class ApplicationUserStoreTests : IntegrationTestBase
//    //{
//    //    private UserManager<ApplicationUser> _userManager = null!;

//    //    public override async Task InitializeAsync()
//    //    {
//    //        await base.InitializeAsync();
//    //        _userManager = BuildUserManager(DbContext);
//    //    }

//    //    public override async Task DisposeAsync()
//    //    {
//    //        await ResetDatabaseAsync();
//    //        await base.DisposeAsync();
//    //    }

//    //    private static UserManager<ApplicationUser> BuildUserManager(AppDbContext db)
//    //    {
//    //        var store = new UserStore<ApplicationUser>(db);

//    //        return new UserManager<ApplicationUser>(
//    //            store,
//    //            null!,
//    //            new PasswordHasher<ApplicationUser>(),
//    //            new IUserValidator<ApplicationUser>[] { new UserValidator<ApplicationUser>() },
//    //            new IPasswordValidator<ApplicationUser>[] { new PasswordValidator<ApplicationUser>() },
//    //            new UpperInvariantLookupNormalizer(),
//    //            new IdentityErrorDescriber(),
//    //            null!,
//    //            NullLogger<UserManager<ApplicationUser>>.Instance);
//    //    }

//    //    // ── Tests ─────────────────────────────────────────────────────────────────

//    //    [Fact]
//    //    public async Task CreateAsync_persists_identity_user_with_hashed_password()
//    //    {
//    //        var identityUser = new ApplicationUser
//    //        {
//    //            Id = Guid.NewGuid().ToString(),
//    //            UserName = "hash@beddin.ng",
//    //            Email = "hash@beddin.ng",
//    //            EmailConfirmed = false,
//    //            IsActive = false,
//    //            FailedLoginAttempts = 0
//    //        };

//    //        var result = await _userManager.CreateAsync(identityUser, "SecureP@ss1");

//    //        result.Succeeded.Should().BeTrue();

//    //        var loaded = await _userManager.FindByEmailAsync("hash@beddin.ng");
//    //        loaded.Should().NotBeNull();
//    //        loaded!.PasswordHash.Should().NotBe("SecureP@ss1");
//    //        loaded.PasswordHash.Should().NotBeNullOrEmpty();
//    //    }

//    //    [Fact]
//    //    public async Task CreateAsync_stores_identity_user_id_matching_domain_user_id()
//    //    {
//    //        var domainUser = User.Create("Tony", "Stark", UserRole.Buyer, "sync@beddin.ng");

//    //        var identityUser = new ApplicationUser
//    //        {
//    //            Id = domainUser.Id.Value.ToString(),
//    //            UserName = "sync@beddin.ng",
//    //            Email = "sync@beddin.ng",
//    //            EmailConfirmed = false,
//    //            IsActive = false,
//    //            FailedLoginAttempts = 0
//    //        };

//    //        var result = await _userManager.CreateAsync(identityUser, "SecureP@ss1");
//    //        result.Succeeded.Should().BeTrue();

//    //        var loaded = await _userManager.FindByEmailAsync("sync@beddin.ng");
//    //        loaded!.Id.Should().Be(domainUser.Id.Value.ToString());
//    //    }

//    //    [Fact]
//    //    public async Task AddClaimAsync_persists_role_claim_correctly()
//    //    {
//    //        var identityUser = new ApplicationUser
//    //        {
//    //            Id = Guid.NewGuid().ToString(),
//    //            UserName = "claim@beddin.ng",
//    //            Email = "claim@beddin.ng",
//    //            EmailConfirmed = false,
//    //            IsActive = false,
//    //            FailedLoginAttempts = 0
//    //        };

//    //        await _userManager.CreateAsync(identityUser, "SecureP@ss1");
//    //        await _userManager.AddClaimAsync(identityUser,
//    //            new System.Security.Claims.Claim("role", "Seeker"));

//    //        var claims = await _userManager.GetClaimsAsync(identityUser);
//    //        claims.Should().ContainSingle(c => c.Type == "role" && c.Value == "Seeker");
//    //    }

//    //    [Fact]
//    //    public async Task DeleteAsync_removes_identity_user_and_validates_compensation_works()
//    //    {
//    //        var identityUser = new ApplicationUser
//    //        {
//    //            Id = Guid.NewGuid().ToString(),
//    //            UserName = "delete@beddin.ng",
//    //            Email = "delete@beddin.ng",
//    //            EmailConfirmed = false,
//    //            IsActive = false,
//    //            FailedLoginAttempts = 0
//    //        };

//    //        await _userManager.CreateAsync(identityUser, "SecureP@ss1");
//    //        await _userManager.DeleteAsync(identityUser);

//    //        var loaded = await _userManager.FindByEmailAsync("delete@beddin.ng");
//    //        loaded.Should().BeNull();
//    //    }
//    //}
//}
