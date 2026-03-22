using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Infrastructure.Persistence;
using Beddin.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;


namespace Beddin.IntegrationTests.Infrastructure.Repositories
{
    public class UserRepositoryTests : IntegrationTestBase
    {
        private readonly IUserRepository _repo;

        public UserRepositoryTests()
        {
            _repo = new UserRepository(DbContext);
        }

        [Fact]
        public async Task AddAsync_persists_domain_user_with_correct_fields()
        {
            var user = User.Create("Tony", "Stark", UserRole.Buyer, "tony@beddin.ng");
            await _repo.AddAsync(user, default);
            await DbContext.SaveChangesAsync();

            var loaded = await DbContext.AppUsers.FindAsync(user.Id);
            loaded.Should().NotBeNull();
            loaded!.FirstName.Should().Be("Tony");
            loaded.LastName.Should().Be("Stark");
            loaded.Email.Should().Be("tony@beddin.ng");
            loaded.Role.Should().Be(UserRole.Buyer);
            loaded.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task AddAsync_persists_strongly_typed_id_correctly()
        {
            var user = User.Create("Tony", "Stark", UserRole.Buyer, "tony2@beddin.ng");
            await _repo.AddAsync(user, default);
            await DbContext.SaveChangesAsync();

            var raw = await DbContext.AppUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == "tony2@beddin.ng");

            raw.Should().NotBeNull();
            raw!.Id.Value.Should().NotBeEmpty();
            raw.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetByEmail_returns_user_when_email_matches()
        {
            var user = User.Create("Tony", "Stark", UserRole.Buyer, "getbyemail@beddin.ng");
            await _repo.AddAsync(user, default);
            await DbContext.SaveChangesAsync();

            var result = await _repo.GetByEmail("getbyemail@beddin.ng", default);
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetByEmail_returns_null_for_unknown_email()
        {
            var result = await _repo.GetByEmail("ghost@beddin.ng", default);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmail_is_case_insensitive()
        {
            var user = User.Create("Tony", "Stark", UserRole.Buyer, "case@beddin.ng");
            await _repo.AddAsync(user, default);
            await DbContext.SaveChangesAsync();

            var result = await _repo.GetByEmail("CASE@BEDDIN.NG", default);
            result.Should().NotBeNull();
        }
    }


    //public class UserRepositoryTests : IntegrationTestBase
    //{
    //    private IUserRepository _repo = null!;

    //    public override async Task InitializeAsync()
    //    {
    //        await base.InitializeAsync();           // spins up Postgres + migrations
    //        _repo = new UserRepository(DbContext);
    //    }

    //    // Reset between tests so state doesn't bleed
    //    public override async Task DisposeAsync()
    //    {
    //        await ResetDatabaseAsync();
    //        await base.DisposeAsync();
    //    }
    //    public UserRepositoryTests()
    //    {
    //        _repo = new UserRepository(DbContext);
    //    }

    //    [Fact]
    //    public async Task AddAsync_persists_domain_user_with_correct_fields()
    //    {
    //        var user = User.Create("Tony", "Stark", UserRole.Buyer, "tony@beddin.ng");

    //        await _repo.AddAsync(user, default);
    //        await DbContext.SaveChangesAsync();

    //        var loaded = await DbContext.AppUsers.FindAsync(user.Id);

    //        loaded.Should().NotBeNull();
    //        loaded!.FirstName.Should().Be("Tony");
    //        loaded.LastName.Should().Be("Stark");
    //        loaded.Email.Should().Be("tony@beddin.ng");
    //        loaded.Role.Should().Be(UserRole.Buyer);
    //        loaded.IsActive.Should().BeFalse();
    //    }

    //    [Fact]
    //    public async Task AddAsync_persists_strongly_typed_id_correctly()
    //    {
    //        var user = User.Create("Tony", "Stark", UserRole.Buyer, "tony2@beddin.ng");

    //        await _repo.AddAsync(user, default);
    //        await DbContext.SaveChangesAsync();

    //        // Query raw to confirm the value converter round-trips the ID
    //        var raw = await DbContext.AppUsers
    //            .AsNoTracking()
    //            .FirstOrDefaultAsync(u => u.Email == "tony2@beddin.ng");

    //        raw.Should().NotBeNull();
    //        raw!.Id.Value.Should().NotBeEmpty();
    //        raw.Id.Should().Be(user.Id);
    //    }

    //    // ?? GetByEmail ????????????????????????????????????????????????????????????

    //    [Fact]
    //    public async Task GetByEmail_returns_user_when_email_matches()
    //    {
    //        var user = User.Create("Tony", "Stark", UserRole.Buyer, "getbyemail@beddin.ng");
    //        await _repo.AddAsync(user, default);
    //        await DbContext.SaveChangesAsync();

    //        var result = await _repo.GetByEmail("getbyemail@beddin.ng", default);

    //        result.Should().NotBeNull();
    //        result!.Id.Should().Be(user.Id);
    //    }

    //    [Fact]
    //    public async Task GetByEmail_returns_null_for_unknown_email()
    //    {
    //        var result = await _repo.GetByEmail("ghost@beddin.ng", default);
    //        result.Should().BeNull();
    //    }

    //    [Fact]
    //    public async Task GetByEmail_is_case_insensitive()
    //    {
    //        var user = User.Create("Tony", "Stark", UserRole.Buyer, "case@beddin.ng");
    //        await _repo.AddAsync(user, default);
    //        await DbContext.SaveChangesAsync();

    //        var result = await _repo.GetByEmail("CASE@BEDDIN.NG", default);
    //        result.Should().NotBeNull();
    //    }
    //}
}