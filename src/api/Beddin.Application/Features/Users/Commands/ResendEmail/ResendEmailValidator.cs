using Beddin.Application.Features.Users.Commands.RegisterUser;
using Beddin.Domain.Aggregates.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.ResendEmail
{
    public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
    {
        public ResendConfirmationEmailValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }
}
