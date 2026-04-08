// <copyright file="DeactivateUserHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Features.Admin.Commands.DeactivateUser
{
    /// <summary>
    /// Handles the deactivation of a user by email.
    /// </summary>
    public sealed class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivateUserHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public DeactivateUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the deactivation request for a user.
        /// </summary>
        /// <param name="request">The deactivation command request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> indicating the result of the operation.
        /// </returns>
        public async Task<ApiResponse<bool>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
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

            var result = user.Deactivate();

            if (!result.IsSuccess)
            {
                return ApiResponse<bool>.Fail(result.Error);
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "User deactivated successfully!");
        }
    }
}
