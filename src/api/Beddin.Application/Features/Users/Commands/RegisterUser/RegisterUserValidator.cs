// <copyright file="RegisterUserValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Users.Commands.RegisterUser
{
    /// <summary>
    /// Validator for the <see cref="RegisterCommand"/>.
    /// </summary>
    public class RegisterUserValidator : AbstractValidator<RegisterCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUserValidator"/> class.
        /// </summary>
        public RegisterUserValidator()
        {
            this.RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            this.RuleFor(x => x.FirstName)
               .NotEmpty()
               .MinimumLength(2).WithMessage("First Name must be at least 2 characters.")
               .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.")
               .WithMessage("First Name is required");

            this.RuleFor(x => x.LastName)
               .NotEmpty()
               .MinimumLength(2).WithMessage("Last Name must be at least 2 characters.")
               .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.")
               .WithMessage("Last Name is required");

            this.RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            this.RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role UUID is required.");
        }
    }
}
