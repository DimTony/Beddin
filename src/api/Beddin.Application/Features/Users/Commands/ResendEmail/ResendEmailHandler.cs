// <copyright file="ResendEmailHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Beddin.Application.Features.Users.Commands.ResendEmail
{
    /// <summary>
    /// Handler for the <see cref="ResendConfirmationEmailCommand"/>.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public sealed class ResendConfirmationEmailHandler : IRequestHandler<ResendConfirmationEmailCommand, ApiResponse<bool>>
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly ILogger<ResendConfirmationEmailHandler> logger;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResendConfirmationEmailHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public ResendConfirmationEmailHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ILogger<ResendConfirmationEmailHandler> logger,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("If an account exists with this email, a verification code has been sent.");
            }

            user.GenerateEmailConfirmationToken();

            var role = await this.roleRepository.GetByIdAsync(user.RoleId, cancellationToken);

            if (role == null)
            {
                this.logger.LogError("Role with ID {RoleId} not found for user {UserId}", user.Role, user.Id);
                return ApiResponse<bool>.Fail("An error occurred while processing your request. Please try again later.");
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);
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
