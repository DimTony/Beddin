// <copyright file="GetUsersQuery.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Admin.Queries.GetUsers
{
    /// <summary>
    /// Query for retrieving users with optional filters and pagination.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to filter by.</param>
    /// <param name="FirstName">The first name to filter users by.</param>
    /// <param name="LastName">The last name to filter users by.</param>
    /// <param name="Email">The email address to filter users by.</param>
    /// <param name="RoleId">The role identifier to filter users by.</param>
    /// <param name="Page">The page number for pagination. Defaults to 1.</param>
    /// <param name="PageSize">The number of users per page. Defaults to 20.</param>
    public sealed record GetUsersQuery(
       Guid? UserId,
       string? FirstName,
       string? LastName,
       string? Email,
       Guid? RoleId,
       int Page = 1,
       int PageSize = 20) : IRequest<PagedResponse<UserDto>>, IRequiresFeature
    {
        /// <inheritdoc/>
        public string FeatureFlag => FeatureFlags.AdminPanel;
    }
}
