// <copyright file="LogoutCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.Logout
{
    /// <summary>
    /// Command to log out the current user.
    /// </summary>
    /// <param name="LogoutAllSessions">Indicates whether to log out all sessions for the user.</param>
    public sealed record LogoutCommand(
        bool LogoutAllSessions = false) : IRequest<ApiResponse<bool>>;

#pragma warning disable SA1402 // File may only contain a single type
    /// <summary>
    /// Command to log out all sessions for a user.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to logout.</param>
    public sealed record LogoutAllSessionsCommand(
#pragma warning restore SA1402 // File may only contain a single type
        string UserId) : IRequest<Result>;
}
