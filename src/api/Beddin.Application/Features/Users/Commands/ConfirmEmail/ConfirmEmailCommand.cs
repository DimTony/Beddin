using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.Login;
using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    public sealed record ConfirmEmailCommand(
      string Email,
      string Token,
      string? IpAddress = null,
      string? UserAgent = null
    ) : IRequest<ApiResponse<LoginResponse>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;
    }
}
