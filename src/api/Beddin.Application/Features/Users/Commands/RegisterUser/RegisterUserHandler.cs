using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Exceptions;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;


namespace Beddin.Application.Features.Users.Commands.RegisterUser
{
    public sealed class RegisterHandler : IRequestHandler<RegisterCommand, ApiResponse<bool>>
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

        public async Task<ApiResponse<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userRepository.GetByEmail(normalizedEmail, cancellationToken);

            if (existingUser != null)
            {
                if (!existingUser.EmailConfirmed)
                {
                    var validRole = await _roleRepository.GetByIdAsync(new RoleId(request.Role), cancellationToken);

                    if (validRole == null)
                        return ApiResponse<bool>.Ok(
                            true,
                            "Check your email for a confirmation link shortly.");

                    var existingUserPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    existingUser.ResendConfirmationToken(request.FirstName, request.LastName, validRole.Id, existingUserPasswordHash, request.Email);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return ApiResponse<bool>.Ok(
                    true,
                    "Check your email for a confirmation link shortly.");

            }

            var role = await _roleRepository.GetByIdAsync(new RoleId(request.Role), cancellationToken);

            if (role == null)
                return ApiResponse<bool>.Ok(
                   true,
                   "Check your email for a confirmation link shortly.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = User.Create(request.FirstName, request.LastName, role.Id, passwordHash, request.Email);

            user.GenerateEmailConfirmationToken();

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(
                   true,
                   "Check your email for a confirmation link shortly.");
        }
    }
}
