using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.CreateRole
{
    public sealed record CreateRoleCommand(
      string Name,
      string Description,
      string? IpAddress = null,
      string? UserAgent = null
    ) : IRequest<ApiResponse<RoleDto>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;
    }

    public sealed record RoleDto(
      Guid Id,
      string Name,
      string Description,
      DateTime CreatedAt);
}
