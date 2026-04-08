// <copyright file="ResendEmailValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Users.Commands.ResendEmail
{
    /// <summary>
    /// Validator for the <see cref="ResendConfirmationEmailCommand"/>.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResendConfirmationEmailValidator"/> class.
        /// </summary>
        public ResendConfirmationEmailValidator()
        {
            this.RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }
}
