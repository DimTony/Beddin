// <copyright file="CreateRoleValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Users.Commands.CreateRole
{
    /// <summary>
    /// Provides validation rules for the <see cref="CreateRoleCommand"/>.
    /// </summary>
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRoleValidator"/> class.
        /// </summary>
        public CreateRoleValidator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            this.RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(3).WithMessage("Description must be at least 3 characters.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        }
    }
}
