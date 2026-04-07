// <copyright file="ActivateUserValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Admin.Commands.ActivateUser
{
    /// <summary>
    /// The <c>ActivateUserValidator</c> class is responsible for validating the properties of the <see cref="ActivateUserCommand"/> to ensure that the data provided for activating a user is valid. It uses FluentValidation to define rules for the command's properties, such as ensuring that the email is not empty, is a valid email address, and falls within specified length constraints.
    /// </summary>
    public class ActivateUserValidator : AbstractValidator<ActivateUserCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivateUserValidator"/> class.
        /// </summary>
        public ActivateUserValidator()
        {
            this.RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }
}
