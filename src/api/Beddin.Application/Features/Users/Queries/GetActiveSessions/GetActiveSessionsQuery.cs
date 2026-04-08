// <copyright file="GetActiveSessionsQuery.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetActiveSessions
{
    /// <summary>
    /// Query definition for getting active sessions of a user using their userId identifier.
    /// </summary>
    /// <param name="UserId">The unique ID of the user whose sessions are being fetched.</param>
    public sealed record GetActiveSessionsQuery(
        string UserId) : IRequest<Result<IEnumerable<SessionDto>>>;
}
