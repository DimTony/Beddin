using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.Login;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.DeleteUser
{
    public sealed record DeleteUserCommand(
       string Email,
       string? IpAddress = null,
       string? UserAgent = null
    ) : IRequest<ApiResponse<LoginResponse>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;
    }
    
}
