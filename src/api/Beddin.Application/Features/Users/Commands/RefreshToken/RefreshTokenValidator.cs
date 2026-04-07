// <copyright file="RefreshTokenValidator.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using FluentValidation;

namespace Beddin.Application.Features.Users.Commands.RefreshToken
{
    /// <summary>
    /// Validator for the <see cref="RefreshTokenCommand"/>.
    /// </summary>
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenValidator"/> class.
        /// </summary>
        public RefreshTokenValidator()
        {
            this.RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
