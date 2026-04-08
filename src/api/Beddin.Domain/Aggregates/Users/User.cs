// <copyright file="User.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Security.Cryptography;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public sealed class User : AggregateRoot<UserId>
    {
        private readonly List<Property> listings = new();
        private readonly List<SavedSearch> savedSearches = new();
        private readonly List<Inquiry> sentInquiries = new();
        private readonly List<Inquiry> receivedInquiries = new();

#pragma warning disable CS0649
        private Role? role;
#pragma warning restore CS0649

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        private User()
        {
        }

        /// <summary>
        /// Gets the first name of the user.
        /// </summary>
        public string FirstName { get; private set; } = default!;

        /// <summary>
        /// Gets the last name of the user.
        /// </summary>
        public string LastName { get; private set; } = default!;

        /// <summary>
        /// Gets the email address of the user.
        /// </summary>
        public string Email { get; private set; } = default!;

        /// <summary>
        /// Gets the password hash of the user.
        /// </summary>
        public string PasswordHash { get; private set; } = default!;

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string? Username { get; private set; }

        /// <summary>
        /// Gets the role identifier of the user.
        /// </summary>
        public RoleId RoleId { get; private set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the email is confirmed.
        /// </summary>
        public bool EmailConfirmed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user must change password.
        /// </summary>
        public bool MustChangePassword { get; set; } = false;

        /// <summary>
        /// Gets the email confirmation token.
        /// </summary>
        public string? EmailConfirmationToken { get; private set; }

        /// <summary>
        /// Gets the email confirmation token expiry date.
        /// </summary>
        public DateTime? EmailConfirmationTokenExpiry { get; private set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token expiry date.
        /// </summary>
        public DateTime? RefreshTokenExpiry { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts.
        /// </summary>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// Gets or sets the date and time until which the user is locked out.
        /// </summary>
        public DateTime? LockedOutUntil { get; set; }

        /// <summary>
        /// Gets the date and time of the last login.
        /// </summary>
        public DateTime LastLoginAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the user was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Gets the role of the user.
        /// </summary>
        public Role Role => this.role!;

        /// <summary>
        /// Gets the property listings owned by the user.
        /// </summary>
        public IReadOnlyCollection<Property> Listings => this.listings;

        /// <summary>
        /// Gets the saved searches created by the user.
        /// </summary>
        public IReadOnlyCollection<SavedSearch> SavedSearches => this.savedSearches;

        /// <summary>
        /// Gets the inquiries sent by the user.
        /// </summary>
        public IReadOnlyCollection<Inquiry> SentInquiries => this.sentInquiries;

        /// <summary>
        /// Gets the inquiries received by the user.
        /// </summary>
        public IReadOnlyCollection<Inquiry> ReceivedInquiries => this.receivedInquiries;

        /// <summary>
        /// Creates a new <see cref="User"/> instance.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="role">The role identifier.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <param name="email">The email address.</param>
        /// <returns>A new <see cref="User"/> instance.</returns>
        public static User Create(
            string firstName,
            string lastName,
            RoleId role,
            string passwordHash,
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.", nameof(email));
            }

            var user = new User
            {
                Id = UserId.New(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHash,
                RoleId = role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            user.RaiseDomainEvent(new UserCreatedEvent(
                    user.Id,
                    firstName,
                    lastName,
                    email,
                    role));

            return user;
        }

        /// <summary>
        /// Records a failed login attempt.
        /// </summary>
        /// <param name="now">The current date and time.</param>
        public void RecordFailedLoginAttempt(DateTime now)
        {
            this.FailedLoginAttempts++;
            if (this.FailedLoginAttempts >= 3)
            {
                this.LockedOutUntil = now.AddMinutes(30);
                this.RaiseDomainEvent(new UserLockedOutEvent(
                    this.Id,
                    this.FirstName,
                    this.LastName,
                    this.Email,
                    this.LockedOutUntil.Value));
            }
        }

        /// <summary>
        /// Attempts to login the user.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="refreshTokenExpiry">The refresh token expiry date.</param>
        /// <param name="now">The current date and time.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result AttemptLogin(string refreshToken, DateTime refreshTokenExpiry, DateTime now)
        {
            if (this.LockedOutUntil.HasValue && this.LockedOutUntil <= now)
            {
                this.LockedOutUntil = null;
                this.FailedLoginAttempts = 0;
            }

            if (!this.EmailConfirmed)
            {
                return Result.Failure("Email not confirmed. Please check your email for confirmation link.");
            }

            if (!this.IsActive)
            {
                return Result.Failure("Account is deactivated. Please contact support.");
            }

            this.RefreshToken = refreshToken;
            this.RefreshTokenExpiry = refreshTokenExpiry;
            this.FailedLoginAttempts = 0;
            this.LockedOutUntil = null;
            this.LastLoginAt = now;
            this.UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        /// <summary>
        /// Sets the refresh token for the user.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="refreshTokenExpiry">The refresh token expiry date.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result SetRefreshToken(string refreshToken, DateTime refreshTokenExpiry)
        {
            this.RefreshToken = refreshToken;
            this.RefreshTokenExpiry = refreshTokenExpiry;
            this.UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        /// <summary>
        /// Resends the confirmation token to the user.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="role">The role identifier.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <param name="email">The email address.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result ResendConfirmationToken(
            string firstName,
            string lastName,
            RoleId role,
            string passwordHash,
            string email)
        {
            if (this.EmailConfirmed)
            {
                return Result.Failure("Email is already confirmed.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.", nameof(email));
            }

            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.RoleId = role;
            this.EmailConfirmationToken = GenerateSecureToken();
            this.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddMinutes(10);

            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new EmailConfirmationTokenGeneratedEvent(
                this.Id, this.FirstName, this.LastName, this.Email, this.EmailConfirmationToken));

            return Result.Success();
        }

        /// <summary>
        /// Generates an email confirmation token.
        /// </summary>
        /// <returns>A result indicating success or failure.</returns>
        public Result GenerateEmailConfirmationToken()
        {
            if (this.EmailConfirmed)
            {
                return Result.Failure("Email is already confirmed.");
            }

            this.EmailConfirmationToken = GenerateSecureToken();
            this.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new EmailConfirmationTokenGeneratedEvent(
                this.Id, this.FirstName, this.LastName, this.Email, this.EmailConfirmationToken));

            return Result.Success();
        }

        /// <summary>
        /// Confirms the email and activates the user account.
        /// </summary>
        /// <param name="token">The confirmation token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="refreshTokenExpiry">The refresh token expiry date.</param>
        /// <param name="now">The current date and time.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result ConfirmEmailAndActivate(
            string token,
            string refreshToken,
            DateTime refreshTokenExpiry,
            DateTime now)
        {
            if (this.EmailConfirmed)
            {
                return Result.Failure("Email is already confirmed.");
            }

            if (this.EmailConfirmationToken != token)
            {
                return Result.Failure("Invalid email confirmation token.");
            }

            if (this.EmailConfirmationTokenExpiry < DateTime.UtcNow)
            {
                return Result.Failure("Email confirmation token has expired.");
            }

            this.EmailConfirmed = true;
            this.IsActive = true;
            this.EmailConfirmationToken = null;
            this.EmailConfirmationTokenExpiry = null;

            this.RefreshToken = refreshToken;
            this.RefreshTokenExpiry = refreshTokenExpiry;
            this.FailedLoginAttempts = 0;
            this.LockedOutUntil = null;
            this.LastLoginAt = now;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new EmailConfirmedEvent(this.Id, this.FirstName, this.LastName, this.Email));

            return Result.Success();
        }

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="newPasswordHash">The new password hash.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result ChangePassword(string newPasswordHash)
        {
            this.PasswordHash = newPasswordHash;
            this.RefreshToken = null;
            this.RefreshTokenExpiry = null;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new PasswordChangedEvent(this.Id, this.FirstName, this.LastName, this.Email));

            return Result.Success();
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        /// <param name="newPasswordHash">The new password hash.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result ResetPassword(string newPasswordHash)
        {
            this.PasswordHash = newPasswordHash;
            this.RefreshToken = null;
            this.RefreshTokenExpiry = null;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new PasswordChangedEvent(this.Id, this.FirstName, this.LastName, this.Email));

            return Result.Success();
        }

        /// <summary>
        /// Updates the user's email address.
        /// </summary>
        /// <param name="newEmail">The new email address.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result UpdateEmail(string newEmail)
        {
            if (this.Email == newEmail)
            {
                return Result.Failure("Cannot set new email as previous email");
            }

            this.Email = newEmail;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new EmailUpdatedEvent(this.Id, this.FirstName, this.LastName, newEmail));

            return Result.Success();
        }

        /// <summary>
        /// Updates the user's role.
        /// </summary>
        /// <param name="newRole">The new role identifier.</param>
        /// <returns>A result indicating success or failure.</returns>
        public Result UpdateRole(RoleId newRole)
        {
            this.RoleId = newRole;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new RoleUpdatedEvent(this.Id, this.FirstName, this.LastName, this.Email, newRole));

            return Result.Success();
        }

        /// <summary>
        /// Deactivates the user account.
        /// </summary>
        /// <returns>A result indicating success or failure.</returns>
        public Result Deactivate()
        {
            this.IsActive = false;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new UserDeactivatedEvent(this.Id, this.FirstName, this.LastName, this.Email));

            return Result.Success();
        }

        /// <summary>
        /// Activates the user account.
        /// </summary>
        /// <returns>A result indicating success or failure.</returns>
        public Result Activate()
        {
            this.IsActive = true;
            this.UpdatedAt = DateTime.UtcNow;

            this.RaiseDomainEvent(new UserActivatedEvent(this.Id, this.FirstName, this.LastName, this.Email));

            return Result.Success();
        }

        /// <summary>
        /// Generates a secure token using cryptographic random number generator.
        /// </summary>
        /// <returns>A secure token string.</returns>
        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }
    }
}
