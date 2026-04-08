// <copyright file="ChangePasswordCommand.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.ChangePassword
{
    /// <summary>
    /// Represents a command to change the password for a user.
    /// </summary>
    /// <param name="CurrentPassword">The user's current password.</param>
    /// <param name="NewPassword">The new password to set.</param>
    /// <param name="ConfirmPassword">The confirmation of the new password.</param>
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword) : IRequest<ApiResponse<string>>;
}
