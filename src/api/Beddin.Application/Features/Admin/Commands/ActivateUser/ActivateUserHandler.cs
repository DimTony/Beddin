// <copyright file="ActivateUserHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Admin.Commands.ActivateUser
{
    /// <summary>
    /// Handles the activation of a user by email.
    /// </summary>
    public sealed class ActivateUserHandler : IRequestHandler<ActivateUserCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IResetPasswordRepository resetPasswordRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivateUserHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="resetPasswordRepository">The reset password repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public ActivateUserHandler(
            IUserRepository userRepository,
            IResetPasswordRepository resetPasswordRepository,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.resetPasswordRepository = resetPasswordRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the activate user command.
        /// </summary>
        /// <param name="request">The activate user command request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating the result of the operation.</returns>
        public async Task<ApiResponse<bool>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return ApiResponse<bool>.Fail("Email is required.");
            }

            var user = await this.userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("Invalid email.");
            }

            var result = user.Activate();

            if (!result.IsSuccess)
            {
                return ApiResponse<bool>.Fail(result.Error);
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "User activated successfully!");
        }
    }
}
