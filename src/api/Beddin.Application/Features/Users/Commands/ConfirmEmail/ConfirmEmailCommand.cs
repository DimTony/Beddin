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
      string UserId,
      string Token
    ) : IRequest<Result>;
}
