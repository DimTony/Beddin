using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.Login
{
    public sealed record LoginCommand(
        string Email,
        string Password
    ) : ApiRequest, IRequest<ApiResponse<LoginResponse>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;

        //public string RateLimitKey => $"login:{Email}";
        //public int MaxAttempts => 5;
        //public int WindowSeconds => 60;
    }

    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken
    );
}
