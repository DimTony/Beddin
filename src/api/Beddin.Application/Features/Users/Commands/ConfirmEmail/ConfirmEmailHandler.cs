using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            if (user.EmailConfirmed)
            {
                return Result.Failure("Email already confirmed.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure($"Email confirmation failed: {errors}");
            }

            return Result.Success();
        }
    }
}
