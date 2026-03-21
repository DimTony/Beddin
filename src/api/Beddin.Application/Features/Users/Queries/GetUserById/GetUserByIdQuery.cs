using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Queries.GetUserById
{
    public sealed record GetUserByIdQuery(UserId UserId) : IRequest<Result<UserDto>>;

}
