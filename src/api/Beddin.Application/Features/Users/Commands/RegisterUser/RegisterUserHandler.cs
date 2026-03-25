using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Exceptions;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Beddin.Application.Features.Users.Commands.RegisterUser
{
    public sealed class RegisterHandler : IRequestHandler<RegisterCommand, ApiResponse<UserDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterHandler(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userRepository.GetByEmail(normalizedEmail, cancellationToken);

            if (existingUser != null)
                return ApiResponse<UserDto>.Fail("User with this email already exists.");

            var role = await _roleRepository.GetByIdAsync(new RoleId(request.Role), cancellationToken);

            if (role == null)
                return ApiResponse<UserDto>.Fail("Invalid role specified.");

            var user = User.Create(request.FirstName, request.LastName, role.Id, request.Password, request.Email);

            user.GenerateEmailConfirmationToken();

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = new UserDto(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email,
                role.Name,
                user.IsActive,
                user.CreatedAt);

            return ApiResponse<UserDto>.Ok(
                userDto,
                "Registration successful. Please check your email to confirm your account.");
        }
    }
}
