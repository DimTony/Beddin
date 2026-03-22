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

namespace Beddin.Application.Features.Users.Commands.RegisterUser
{
    public sealed record RegisterCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role
    ) : IRequest<ApiResponse<UserDto>>,  IRequiresFeature
    {
        public string FeatureFlag => FeatureFlags.Authentication;
    }


    public sealed record UserRegistrationPayload(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role
    );

    public sealed record UserDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAt
    );

    //

    //public record CreateUserCommand(
    //string Username,
    //string Email,
    //string FirstName,
    //string LastName,
    //string Password,
    //string Role,
    //Guid? MemberId) : IRequest<Result<Guid>>, IAuditable, IRequiresFeature
    //{
    //    public string AuditResource => "User";
    //    public Guid? AuditResourceId => null;

    //    public string FeatureFlag => FeatureFlags.UserManagement;
    //}
}
