using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetUserById
{
    public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IRepository<User, UserId> _userRepository;

        public GetUserByIdHandler(IRepository<User, UserId> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserDto>($"User with ID {request.UserId.Value} not found");
            }

            var userDto = new UserDto(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role.ToString(),
                user.IsActive,
                user.CreatedAt
            );

            return Result.Success<UserDto>(userDto);
        }
    }
}
