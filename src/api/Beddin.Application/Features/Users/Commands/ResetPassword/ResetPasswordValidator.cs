// <copyright file="ResetPasswordValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Users.Commands.ResetPassword
{
    /// <summary>
    /// Validator for the <see cref="RequestPasswordResetCommand"/>.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
    public class RequestPasswordResetValidator : AbstractValidator<RequestPasswordResetCommand>
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPasswordResetValidator"/> class.
        /// </summary>
        public RequestPasswordResetValidator()
        {
            this.RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
        }
    }

    /// <summary>
    /// Validator for the <see cref="ResetPasswordCommand"/>.
    /// </summary>
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordValidator"/> class.
        /// </summary>
        public ResetPasswordValidator()
        {
            this.RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Valid email is required.")
               .MinimumLength(3).WithMessage("Email must be at least 3 characters.")
               .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            this.RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");

            this.RuleFor(x => x.NewPassword)
               .NotEmpty().WithMessage("Password is required.")
               .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
               .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
               .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
               .Matches("[0-9]").WithMessage("Password must contain at least one number.")
               .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
    }
}
