// <copyright file="GetUsersDto.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Features.Admin.Queries.GetUsers
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a user in the system, used for transferring user data between layers of the application.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user.</param>
    /// <param name="FirstName">The first name of the user.</param>
    /// <param name="LastName">The last name of the user.</param>
    /// <param name="Email">The email address of the user.</param>
    /// <param name="Role">The role of the user within the system.</param>
    /// <param name="EmailConfirmed">Indicates whether the user's email is confirmed.</param>
    /// <param name="IsActive">Indicates whether the user is currently active.</param>
    /// <param name="LastLoginAtAt">The date and time when the user last logged in.</param>
    /// <param name="CreatedAt">The date and time when the user was created.</param>
#pragma warning disable SA1649 // File name should match first type name
    public sealed record UserDto(
#pragma warning restore SA1649 // File name should match first type name
       Guid UserId,
       string FirstName,
       string LastName,
       string Email,
       string Role,
       bool EmailConfirmed,
       bool IsActive,
       DateTime LastLoginAtAt,
       DateTime CreatedAt);
}
