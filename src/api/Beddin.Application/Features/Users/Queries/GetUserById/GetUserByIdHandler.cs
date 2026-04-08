// <copyright file="GetUserByIdHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetUserById
{
    /// <summary>
    /// Handles the retrieval of a user by their unique identifier.
    /// </summary>
    public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IRepository<User, UserId> userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserByIdHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for managing users.</param>
        public GetUserByIdHandler(IRepository<User, UserId> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Handles the request to retrieve a user by ID.
        /// </summary>
        /// <param name="request">The query containing the user ID.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A result containing the user data transfer object if found; otherwise, a failure result.</returns>
        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserDto>($"User with ID {request.UserId.Value} not found");
            }

            var userDto = new UserDto(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email,
                user.RoleId.ToString(),
                user.IsActive,
                user.CreatedAt);

            return Result.Success<UserDto>(userDto);
        }
    }
}
