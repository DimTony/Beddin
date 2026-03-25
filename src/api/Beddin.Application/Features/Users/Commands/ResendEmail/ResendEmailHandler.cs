using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Beddin.Domain.Aggregates.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net;

namespace Beddin.Application.Features.Users.Commands.ResendEmail
{
    public sealed class ResendConfirmationEmailHandler : IRequestHandler<ResendConfirmationEmailCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<ResendConfirmationEmailHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ResendConfirmationEmailHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ILogger<ResendConfirmationEmailHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
                return ApiResponse<bool>.Fail("If an account exists with this email, a verification code has been sent.");

            user.GenerateEmailConfirmationToken();


            var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
    
            if (role == null)
            {
                _logger.LogError("Role with ID {RoleId} not found for user {UserId}", user.Role, user.Id);
                return ApiResponse<bool>.Fail("An error occurred while processing your request. Please try again later.");
            }
    
            await _unitOfWork.SaveChangesAsync(cancellationToken);
          
            var userDto = new UserDto(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email,
                role.Name,
                user.IsActive,
                user.CreatedAt);

            return ApiResponse<bool>.Ok(true, "If an account exists with this email, a verification code has been sent.");
        }
    }
}
