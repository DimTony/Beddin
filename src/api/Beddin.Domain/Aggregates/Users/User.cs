using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Beddin.Domain.Events;
using Microsoft.AspNetCore.Identity;


namespace Beddin.Domain.Aggregates.Users
{
    public class ApplicationUser : IdentityUser
    {
        //public Guid UserId { get; set; } // Link to DDD User
        // Only auth-related extensions here
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool IsActive { get; set; } = false;

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockedOutUntil { get; set; }
    }
    public sealed class User : AggregateRoot<UserId>
    {
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public UserRole Role { get; private set; }
        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

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
            UserRole role,
            string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var user = new User
            {
                Id = UserId.New(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role,
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

        public void UpdateEmail(string newEmail)
        {
            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new EmailUpdatedEvent(Id, FirstName, LastName, newEmail));
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserDeactivatedEvent(Id, FirstName, LastName, Email));
        }
    }
}
