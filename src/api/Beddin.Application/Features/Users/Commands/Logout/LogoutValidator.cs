// <copyright file="LogoutValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Users.Commands.Logout
{
    /// <summary>
    /// Validator for the <see cref="LogoutCommand"/>.
    /// </summary>
    public class LogoutValidator : AbstractValidator<LogoutCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutValidator"/> class.
        /// </summary>
        public LogoutValidator()
        {
            this.RuleFor(x => x.LogoutAllSessions);
        }
    }
}
