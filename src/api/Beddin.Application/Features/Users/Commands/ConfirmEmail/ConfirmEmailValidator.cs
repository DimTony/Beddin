// <copyright file="ConfirmEmailValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    /// <summary>
    /// Provides validation rules for the <see cref="ConfirmEmailCommand"/>.
    /// </summary>
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmEmailValidator"/> class.
        /// </summary>
        public ConfirmEmailValidator()
        {
            this.RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            this.RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");
        }
    }
}
