using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Infrastructure.Persistence;
using FluentAssertions;
using Xunit;


namespace Beddin.IntegrationTests.Infrastructure.Repositories
{
    public class UserRepositoryTests : IntegrationTestBase
    {
        //private readonly IRepository<User, UserId> _repository;

        //public UserRepositoryTests()
        //{

        //    _repository = new Repository<User, UserId>(DbContext);
        //}
        private IRepository<User, UserId> _repository = null!;
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync(); 
            _repository = new Repository<User, UserId>(DbContext);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistUser_ToDatabase()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                UserRoles.Buyer,
                "test@example.com"
            );

            // Act
            await _repository.AddAsync(user);
            await DbContext.SaveChangesAsync();

            // Assert
            var savedUser = await _repository.GetByIdAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser!.Email.Should().Be("test@example.com");
            savedUser.FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = new UserId(Guid.NewGuid());

            // Act
            var user = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            user.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingUser()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                UserRoles.Buyer,
                "old@example.com"
            );

            if (user is null)
            {
                throw new Exception("Failed to create user");
            }

            await _repository.AddAsync(user);
            await DbContext.SaveChangesAsync();

            // Act
            user.UpdateEmail("new@example.com");

            await _repository.UpdateAsync(user);

            await DbContext.SaveChangesAsync();

            // Assert
            var updatedUser = await _repository.GetByIdAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Email.Should().Be("new@example.com");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser_FromDatabase()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                UserRoles.Buyer,
                "test@example.com"
            );
            await _repository.AddAsync(user);
            await DbContext.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(user);


            await DbContext.SaveChangesAsync();

            // Assert
            var deletedUser = await _repository.GetByIdAsync(user.Id);
            deletedUser.Should().BeNull();
        }
    }
}