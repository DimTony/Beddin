using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.CreateUser;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using FluentAssertions;
using NSubstitute;
using Xunit;



namespace Beddin.UnitTests.Application.Features.Users.Commands
{
    public class CreateUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateUserCommandHandler _handler;
        private readonly IPasswordService _passwordService;

        public CreateUserCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateUserCommandHandler(_userRepository, _unitOfWork);
            _passwordService = Substitute.For<IPasswordService>();
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenEmailIsUnique()
        {
            var tempPassword = _passwordService.GenerateTemporaryPassword(length: 12);

            // Arrange
            var command = new CreateUserCommand(
                "John",
                "Doe",
                "john.doe@example.com",
                "Buyer",
                tempPassword
            );

            _userRepository.GetByEmail(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Should().NotBeNull();

            await _userRepository.Received(1).AddAsync(
                Arg.Is<User>(u =>
                    u.Email == command.Email &&
                    u.FirstName == command.FirstName),
                Arg.Any<CancellationToken>());

            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = User.Create("Jane", "Doe", "Buyer", "john.doe@example.com");

            var tempPassword = _passwordService.GenerateTemporaryPassword(length: 12);

            var command = new CreateUserCommand(
                "John",
                "Doe",
                "john.doe@example.com",
                "Buyer",
                tempPassword
            );

            _userRepository.GetByEmail(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
                .Returns(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("already exists");

            await _userRepository.DidNotReceive().AddAsync(
                Arg.Any<User>(),
                Arg.Any<CancellationToken>());

            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData("", "Doe", "john@example.com")]
        [InlineData("John", "", "john@example.com")]
        [InlineData("John", "Doe", "")]
        public void Validator_ShouldHaveErrors_WhenRequiredFieldsAreMissing(
            string firstName,
            string lastName,
            string email)
        {
            // Arrange
            var command = new CreateUserCommand(firstName, lastName, email, "Buyer", "Password123");
            var validator = new CreateUserValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }
}
