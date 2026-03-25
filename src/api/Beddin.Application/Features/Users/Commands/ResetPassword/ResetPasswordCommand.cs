using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.ResetPassword
{
    public sealed record RequestPasswordResetCommand(
        string Email,
        string IpAddress,
        string UserAgent
    ) : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;
    }

    public sealed record ResetPasswordCommand(
       string Email,
       string Token,
       string NewPassword
   ) : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;
    }
}
