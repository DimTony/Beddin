using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Queries.GetActiveSessions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Queries.GetUsers
{
    public sealed record GetUsersQuery(
       Guid? UserId,
       string? FirstName,
       string? LastName,
       string? Email,
       Guid? RoleId,
       int Page = 1,
       int PageSize = 20
    ) : IRequest<PagedResponse<UserDto>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.AdminPanel;

    }
}
