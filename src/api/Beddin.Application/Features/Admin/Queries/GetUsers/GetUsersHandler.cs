using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Admin.Queries.GetSessions;
using Beddin.Application.Features.Users.Queries.GetActiveSessions;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Queries.GetUsers
{
    public sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pagedUsers = await _userRepository.GetAllUsers(
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
                u.CreatedAt
            )).ToList();

            return PagedResponse<UserDto>.Ok(
                userDtos, request.Page, request.PageSize, pagedUsers.TotalCount, "Users fetched!");
        }
    }
}
