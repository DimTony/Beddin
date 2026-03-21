using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Queries.GetUserById;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Beddin.UnitTests.Application.Features.Users.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly GetUserByIdHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new GetUserByIdHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = UserId.New();
            var user = User.Create("John", "Doe", "Buyer", "john@example.com");
            var query = new GetUserByIdQuery(userId);

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Should().NotBeNull();
            result.Value.Email.Should().Be("john@example.com");
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = UserId.New();
            var query = new GetUserByIdQuery(userId);

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }

        [Fact]
        public async Task Handle_ShouldMapUserCorrectly_ToDto()
        {
            // Arrange
            var userId = UserId.New();
            var user = User.Create("John", "Doe", "Owner", "john@example.com");
            var query = new GetUserByIdQuery(userId);

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Role.Should().Be("Owner");
            result.Value.IsActive.Should().BeTrue();
            result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
}
