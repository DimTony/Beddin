using Beddin.Application.Common.DTOs;
using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.RefreshToken
{
    public sealed record RefreshTokenCommand(
        string RefreshToken,
        string? IpAddress = null,
        string? UserAgent = null
    ) : IRequest<ApiResponse<RefreshTokenResponse>>;

    public sealed record RefreshTokenResponse(
        string AccessToken,
        string RefreshToken
    );
}
