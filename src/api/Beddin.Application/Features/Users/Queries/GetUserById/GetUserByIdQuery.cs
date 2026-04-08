// <copyright file="GetUserByIdQuery.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetUserById
{
    /// <summary>
    /// Represents a query to retrieve a user by their unique identifier.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to retrieve.</param>
    public sealed record GetUserByIdQuery(UserId UserId) : IRequest<Result<UserDto>>;
}
