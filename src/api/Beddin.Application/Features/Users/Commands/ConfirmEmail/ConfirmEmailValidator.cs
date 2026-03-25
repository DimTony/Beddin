using Beddin.Application.Features.Users.Commands.Login;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");
        }
    }
}
