using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Beddin.Application.Features.Users.Commands.Login
{
    public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private const int MaxFailedAttempts = 3;
        private const int LockoutMinutes = 30;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginHandler(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            ITokenService tokenService,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find identity user
            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            if (identityUser == null)
            {
                return Result.Failure<LoginResponse>("Invalid email or password.");
            }

            // Check if account is locked
            if (identityUser.LockedOutUntil.HasValue && identityUser.LockedOutUntil > DateTime.UtcNow)
            {
                var remainingMinutes = (identityUser.LockedOutUntil.Value - DateTime.UtcNow).Minutes;
                return Result.Failure<LoginResponse>($"Account is locked. Try again in {remainingMinutes} minutes.");
            }

            // Reset lockout if expired
            if (identityUser.LockedOutUntil.HasValue && identityUser.LockedOutUntil <= DateTime.UtcNow)
            {
                identityUser.LockedOutUntil = null;
                identityUser.FailedLoginAttempts = 0;
                await _userManager.UpdateAsync(identityUser);
            }

            // Check if email is confirmed
            if (!identityUser.EmailConfirmed)
            {
                return Result.Failure<LoginResponse>("Email not confirmed. Please check your email for confirmation link.");
            }

            // Check if account is active
            if (!identityUser.IsActive)
            {
                return Result.Failure<LoginResponse>("Account is deactivated. Please contact support.");
            }

            // Verify password
            var passwordValid = await _userManager.CheckPasswordAsync(identityUser, request.Password);
            if (!passwordValid)
            {
                // Increment failed attempts
                identityUser.FailedLoginAttempts++;

                if (identityUser.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    identityUser.LockedOutUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    await _userManager.UpdateAsync(identityUser);
                    await _emailService.SendAccountLockedAsync(request.Email);
                    return Result.Failure<LoginResponse>($"Account locked due to {MaxFailedAttempts} failed login attempts. Try again in {LockoutMinutes} minutes.");
                }

                await _userManager.UpdateAsync(identityUser);
                var attemptsRemaining = MaxFailedAttempts - identityUser.FailedLoginAttempts;
                return Result.Failure<LoginResponse>($"Invalid email or password. {attemptsRemaining} attempts remaining.");
            }

            // Reset failed attempts on successful login
            if (identityUser.FailedLoginAttempts > 0)
            {
                identityUser.FailedLoginAttempts = 0;
                await _userManager.UpdateAsync(identityUser);
            }

            // Get domain user
            var userId = new UserId(Guid.Parse(identityUser.Id));
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return Result.Failure<LoginResponse>("User data not found.");
            }

            // Generate refresh token
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // Create session FIRST (before generating access token)
            var session = UserSession.Create(
                userId,
                refreshToken,
                refreshTokenExpiry,
                request.IpAddress,
                request.UserAgent
            );

            await _sessionRepository.Add(session, cancellationToken);

            // Generate access token with session ID embedded
            var accessToken = _tokenService.GenerateAccessToken(user, session.Id.Value);
            var expiresAt = _tokenService.GetTokenExpiration(accessToken);

            // Save refresh token to Identity user
            identityUser.RefreshToken = refreshToken;
            identityUser.RefreshTokenExpiry = refreshTokenExpiry;
            await _userManager.UpdateAsync(identityUser);

            // Commit all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new LoginResponse(
                accessToken, 
                refreshToken, 
                expiresAt,
                session.Id.Value
            ));
        }
    }
    
}
