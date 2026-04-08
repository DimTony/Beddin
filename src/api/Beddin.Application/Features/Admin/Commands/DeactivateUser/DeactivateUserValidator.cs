// <copyright file="DeactivateUserValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Admin.Commands.DeactivateUser
{
    /// <summary>
    /// Provides validation rules for the <see cref="DeactivateUserCommand"/>.
    /// </summary>
    public class DeactivateUserValidator : AbstractValidator<DeactivateUserCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivateUserValidator"/> class.
        /// </summary>
        public DeactivateUserValidator()
        {
            this.RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }
}
