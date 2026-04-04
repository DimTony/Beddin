using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Beddin.Domain.Events;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Cryptography;

namespace Beddin.Domain.Aggregates.Users
{
    public sealed class User : AggregateRoot<UserId>
    {
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string? Username { get; private set; }
        public RoleId RoleId { get; private set; } = default!;
        public bool IsActive { get; set; } = false;
        public bool EmailConfirmed { get; set; } = false;
        public bool MustChangePassword { get; set; } = false;
        public string? EmailConfirmationToken { get; private set; }
        public DateTime? EmailConfirmationTokenExpiry { get; private set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockedOutUntil { get; set; }
        public DateTime LastLoginAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

#pragma warning disable CS0649
        private Role? _role;
#pragma warning restore CS0649
        public Role Role => _role!;

        private readonly List<Property> _listings = new();
        public IReadOnlyCollection<Property> Listings => _listings;

        private readonly List<SavedSearch> _savedSearches = new();
        public IReadOnlyCollection<SavedSearch> SavedSearches => _savedSearches;

        private readonly List<Inquiry> _sentInquiries = new();
        public IReadOnlyCollection<Inquiry> SentInquiries => _sentInquiries;

        private readonly List<Inquiry> _receivedInquiries = new();
        public IReadOnlyCollection<Inquiry> ReceivedInquiries => _receivedInquiries;



        private User() { }

        public static User Create(
            string firstName,
            string lastName,
            RoleId role,
            string passwordHash,
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var user = new User
            {
                Id = UserId.New(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHash,
                RoleId = role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user.RaiseDomainEvent(new UserCreatedEvent(
                    user.Id,
                    firstName,
                    lastName,
                    email,
                    role));

            return user;
        }

        public void RecordFailedLoginAttempt(DateTime now)
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 3)
            {
                LockedOutUntil = now.AddMinutes(30);
                RaiseDomainEvent(new UserLockedOutEvent(
                    Id,
                    FirstName,
                    LastName,
                    Email,
                    LockedOutUntil.Value));
            }
        }

        public Result AttemptLogin(string refreshToken,
            DateTime refreshTokenExpiry, DateTime now)
        {
            if (LockedOutUntil.HasValue && LockedOutUntil <= now)
            {
                LockedOutUntil = null;
                FailedLoginAttempts = 0;
            }

            if (!EmailConfirmed)
                return Result.Failure("Email not confirmed. Please check your email for confirmation link.");

            if (!IsActive)
                return Result.Failure("Account is deactivated. Please contact support.");

            RefreshToken = refreshToken;
            RefreshTokenExpiry = refreshTokenExpiry;
            FailedLoginAttempts = 0;
            LockedOutUntil = null;
            LastLoginAt = now;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result SetRefreshToken(string refreshToken, DateTime refreshTokenExpiry)
        {

            RefreshToken = refreshToken;
            RefreshTokenExpiry = refreshTokenExpiry;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result ResendConfirmationToken(
            string firstName,
            string lastName,
            RoleId role,
            string passwordHash,
            string email)
        {
            if (EmailConfirmed)
                return Result.Failure("Email is already confirmed.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            RoleId = role;

            EmailConfirmationToken = GenerateSecureToken();
            EmailConfirmationTokenExpiry = DateTime.UtcNow.AddMinutes(10);

            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new EmailConfirmationTokenGeneratedEvent(
                Id, FirstName, LastName, Email, EmailConfirmationToken));

            return Result.Success();
        }

        public Result GenerateEmailConfirmationToken()
        {
            if (EmailConfirmed)
                return Result.Failure("Email is already confirmed.");

            EmailConfirmationToken = GenerateSecureToken();
            EmailConfirmationTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new EmailConfirmationTokenGeneratedEvent(
                Id, FirstName, LastName, Email, EmailConfirmationToken));

            return Result.Success();
        }

        public Result ConfirmEmailAndActivate(
            string token,
            string refreshToken,
            DateTime refreshTokenExpiry,
            DateTime now)
        {
            if (EmailConfirmed)
                return Result.Failure("Email is already confirmed.");

            if (EmailConfirmationToken != token)
                return Result.Failure("Invalid email confirmation token.");

            if (EmailConfirmationTokenExpiry < DateTime.UtcNow)
                return Result.Failure("Email confirmation token has expired.");

            EmailConfirmed = true;
            IsActive = true;
            EmailConfirmationToken = null;
            EmailConfirmationTokenExpiry = null;

            RefreshToken = refreshToken;
            RefreshTokenExpiry = refreshTokenExpiry;
            FailedLoginAttempts = 0;
            LockedOutUntil = null;
            LastLoginAt = now;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new EmailConfirmedEvent(Id, FirstName, LastName, Email));

            return Result.Success();
        }

        public Result ChangePassword(string newPasswordHash)
        {


            PasswordHash = newPasswordHash;
            RefreshToken = null;
            RefreshTokenExpiry = null;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PasswordChangedEvent(Id, FirstName, LastName, Email));

            return Result.Success();
        }

        public Result ResetPassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            RefreshToken = null;
            RefreshTokenExpiry = null;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PasswordChangedEvent(Id, FirstName, LastName, Email));

            return Result.Success();
        }

        public Result UpdateEmail(string newEmail)
        {
            if (Email == newEmail)
            {
                return Result.Failure("Cannot set new email as previous email");
            }

            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new EmailUpdatedEvent(Id, FirstName, LastName, newEmail));

            return Result.Success();
        }


        public Result UpdateRole(RoleId newRole)
        {
            if (RoleId == newRole)
            {
                return Result.Failure("Cannot set new role as previous role");
            }

            RoleId = newRole;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new RoleUpdatedEvent(Id, FirstName, LastName, Email, newRole));

            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
            {
                return Result.Success();
            }

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserActivatedEvent(Id, FirstName, LastName, Email));

            return Result.Success();

        }

        public Result Deactivate()
        {
            if (!IsActive)
            {
                return Result.Success();
            }

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserDeactivatedEvent(Id, FirstName, LastName, Email));

            return Result.Success();
        }

        private static string GenerateSecureToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
              .Replace("+", "-")
              .Replace("/", "_")
              .TrimEnd('=');
    }
}
