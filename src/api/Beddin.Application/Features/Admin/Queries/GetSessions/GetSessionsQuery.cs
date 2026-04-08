// <copyright file="GetSessionsQuery.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Queries.GetActiveSessions;
using MediatR;

namespace Beddin.Application.Features.Admin.Queries.GetSessions
{
    /// <summary>
    /// Represents a query to retrieve a paginated list of user sessions, optionally filtered by a specific user ID. This query is intended for administrative use and requires the appropriate feature flag to be enabled.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to filter sessions by. If null, sessions for all users are returned.</param>
    /// <param name="Page">The page number for pagination.</param>
    /// <param name="PageSize">The number of sessions to return per page.</param>
    public sealed record GetSessionsQuery(
       string? UserId,
       int Page = 1,
       int PageSize = 20) : IRequest<PagedResponse<SessionDto>>, IRequiresFeature
    {
        /// <inheritdoc/>
        public string FeatureFlag => FeatureFlags.AdminPanel;
    }
}
