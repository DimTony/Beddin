using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Beddin.Application.Features.Users.Commands.RegisterUser
{
    public sealed class RegisterHandler : IRequestHandler<RegisterCommand, ApiResponse<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterHandler(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // ── Guards ────────────────────────────────────────────────────────────
            var existingAppUser = await _userRepository.GetByEmail(request.Email, cancellationToken);
            var existingIdentityUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingAppUser != null || existingIdentityUser is not null)
                return ApiResponse<UserDto>.Fail("User with this email already exists.");

            if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
                return ApiResponse<UserDto>.Fail("Invalid role specified.");

            // ── Build both users before touching any store ────────────────────────
            var user = User.Create(request.FirstName, request.LastName, userRole, request.Email);

            var identityUser = new ApplicationUser
            {
                Id = user.Id.Value.ToString(),   // keep IDs in sync
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = false,
                IsActive = false,
                FailedLoginAttempts = 0
            };

            // ── Step 1: Identity (can't be inside EF transaction) ────────────────
            var identityResult = await _userManager.CreateAsync(identityUser, request.Password);
            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                return ApiResponse<UserDto>.Fail($"Failed to create user: {errors}");
            }

            // ── Step 2: Domain + claim — roll back identity if anything fails ─────
            try
            {
                await _userManager.AddClaimAsync(identityUser,
                    new System.Security.Claims.Claim("role", userRole.ToString()));

                await _userRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                // Compensate — delete the identity user so we don't leave orphans
                await _userManager.DeleteAsync(identityUser);
                throw;  // let the exception bubble — caller gets a 500, not a silent split
            }

            // ── Return ────────────────────────────────────────────────────────────
            var userDto = new UserDto(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role.ToString(),
                user.IsActive,
                user.CreatedAt);

            return ApiResponse<UserDto>.Ok(userDto, "User registered successfully!");
        }
    }
}
