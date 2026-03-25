using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.ResetPassword;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Features.Admin.Commands.ActivateUser
{
    public sealed class DeactivateUserHandler : IRequestHandler<ActivateUserCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateUserHandler(
            IUserRepository userRepository,
            IResetPasswordRepository resetPasswordRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _resetPasswordRepository = resetPasswordRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {


            if (string.IsNullOrEmpty(request.Email))
                return ApiResponse<bool>.Fail("Email is required.");

            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
                return ApiResponse<bool>.Fail("Invalid email.");

            var result = user.Activate();

            if (!result.IsSuccess)
                return ApiResponse<bool>.Fail(result.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "User activated successfully!");

        }

    }
}
