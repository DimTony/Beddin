using Beddin.Application.Features.Users.Commands.ResetPassword;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Commands.ActivateUser
{
    public class DeactivateUserValidator : AbstractValidator<ActivateUserCommand>
    {
        public DeactivateUserValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }

}
