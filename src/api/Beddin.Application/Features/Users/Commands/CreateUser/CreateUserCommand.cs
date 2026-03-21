using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Users.Commands.CreateUser
{
    public sealed record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Role,
        string Password
    ) : IRequest<Result<UserId>>;

    public record UserRegistrationPayload(
     string FirstName,
     string LastName,
     string Email,
     string Role,
     string Password);
}
