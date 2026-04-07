// <copyright file="RegisterUserHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.RegisterUser
{
    /// <summary>
    /// Handler for the <see cref="RegisterCommand"/>.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public sealed class RegisterHandler : IRequestHandler<RegisterCommand, ApiResponse<bool>>
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterHandler"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public RegisterHandler(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var existingUser = await this.userRepository.GetByEmail(normalizedEmail, cancellationToken);

            if (existingUser != null)
            {
                if (!existingUser.EmailConfirmed)
                {
                    var validRole = await this.roleRepository.GetByIdAsync(new RoleId(request.Role), cancellationToken);

                    if (validRole == null)
                    {
                        return ApiResponse<bool>.Ok(
                            true,
                            "Check your email for a confirmation link shortly.");
                    }

                    var existingUserPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    existingUser.ResendConfirmationToken(request.FirstName, request.LastName, validRole.Id, existingUserPasswordHash, request.Email);

                    await this.unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return ApiResponse<bool>.Ok(
                    true,
                    "Check your email for a confirmation link shortly.");
            }

            var role = await this.roleRepository.GetByIdAsync(new RoleId(request.Role), cancellationToken);

            if (role == null)
            {
                return ApiResponse<bool>.Ok(
                   true,
                   "Check your email for a confirmation link shortly.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = User.Create(request.FirstName, request.LastName, role.Id, passwordHash, request.Email);

            user.GenerateEmailConfirmationToken();

            await this.userRepository.AddAsync(user, cancellationToken);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(
                   true,
                   "Check your email for a confirmation link shortly.");
        }
    }
}
