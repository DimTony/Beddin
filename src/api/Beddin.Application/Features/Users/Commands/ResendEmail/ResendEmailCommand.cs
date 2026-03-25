using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.ResendEmail
{
    public sealed record ResendConfirmationEmailCommand(
        string Email
    ) : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;
    }
    public sealed record ResendConfirmationEmailPayload(
        string Email
    );

    public sealed record UserDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAt
    );
}
