using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Beddin.Application.Features.Users.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;


        public ConfirmEmailHandler(IUserRepository userRepository, IUserSessionRepository sessionRepository, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null || user.EmailConfirmed)
                return ApiResponse<bool>.Fail("Invalid email or token.");

            var decodedToken = WebUtility.UrlDecode(request.Token);

            var result = user.ConfirmEmailToken(decodedToken);
            if (!result.IsSuccess)
            {
                return ApiResponse<bool>.Fail(result.Error);
            }

            user.Activate();

            return ApiResponse<bool>.Ok(true, "Email confirmed successfully!");
        }
    }
}
