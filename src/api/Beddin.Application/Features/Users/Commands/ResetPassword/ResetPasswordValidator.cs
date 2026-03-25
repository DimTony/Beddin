using Beddin.Application.Features.Users.Commands.RegisterUser;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.ResetPassword
{
    public class RequestPasswordResetValidator : AbstractValidator<RequestPasswordResetCommand>
    {
        public RequestPasswordResetValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");

            RuleFor(x => x.NewPassword)
               .NotEmpty().WithMessage("Password is required.")
               .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
               .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
               .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
               .Matches("[0-9]").WithMessage("Password must contain at least one number.")
               .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
    }
}
