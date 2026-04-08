// <copyright file="GetUsersHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Admin.Queries.GetUsers
{
    /// <summary>
    /// Handles the retrieval of paged user data for administrative queries.
    /// </summary>
    public sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResponse<UserDto>>
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUsersHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for managing users.</param>
        public GetUsersHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pagedUsers = await this.userRepository.GetAllUsers(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.RoleId,
                request.Page,
                request.PageSize,
                cancellationToken);

            var userDtos = pagedUsers.Items.Select(u => new UserDto(
                u.Id.Value,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Role.Name,
                u.EmailConfirmed,
                u.IsActive,
                u.LastLoginAt,
                u.CreatedAt)).ToList();

            return PagedResponse<UserDto>.Ok(
                userDtos, request.Page, request.PageSize, pagedUsers.TotalCount, "Users fetched!");
        }
    }
}
