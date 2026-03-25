using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Helpers;
using Beddin.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Commands.DeactivateUser
{
    public sealed record DeactivateUserCommand(
        string Email,
        string IpAddress,
        string UserAgent
    ) : IRequest<ApiResponse<bool>>, IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.AdminPanel;
    }
}
