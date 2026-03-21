using MediatR;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;

namespace Beddin.Application.Features.Users.Commands.CreateUser
{


    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserId>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user with email already exists
            var existingUser = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (existingUser != null)
            {
                return Result.Failure<UserId>($"User with email {request.Email} already exists");
            }

            // Create user
            var user = User.Create(
                request.FirstName,
                request.LastName,
                request.Role,
                request.Email
            );

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success<UserId>(user.Id);
        }
    }
}
