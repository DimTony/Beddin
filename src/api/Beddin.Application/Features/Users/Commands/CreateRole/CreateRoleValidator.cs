using Beddin.Application.Features.Users.Commands.ConfirmEmail;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.CreateRole
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(3).WithMessage("Description must be at least 3 characters.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        }
    }
}
