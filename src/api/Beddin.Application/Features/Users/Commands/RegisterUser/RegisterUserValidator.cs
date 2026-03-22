using Beddin.Domain.Aggregates.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");


            RuleFor(x => x.FirstName)
               .NotEmpty()
               .MinimumLength(2).WithMessage("First Name must be at least 2 characters.")
               .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.")
               .WithMessage("First Name is required");

            RuleFor(x => x.LastName)
               .NotEmpty()
               .MinimumLength(2).WithMessage("Last Name must be at least 2 characters.")
               .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.")
               .WithMessage("Last Name is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(UserRoles.IsValid)
                .WithMessage($"Role must be one of: {string.Join(", ", UserRoles.All)}");
        }
    }
}
