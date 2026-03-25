using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.ResetPassword;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Commands.DeactivateUser
{
    public sealed class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {


            if (string.IsNullOrEmpty(request.Email))
                return ApiResponse<bool>.Fail("Email is required.");

            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
                return ApiResponse<bool>.Fail("Invalid email.");

            var result = user.Deactivate();

            if (!result.IsSuccess)
                return ApiResponse<bool>.Fail(result.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "User deactivated successfully!");

        }

    }
}
