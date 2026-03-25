using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Common;
using Beddin.Domain.Events;
using System;

namespace Beddin.Domain.Aggregates.Users
{
    public sealed class SavedSearch : AggregateRoot<SavedSearchId>
    {
        public UserId UserId { get; private set; } = null!;
        public string Name { get; private set; } = null!;

        public PropertyType PropertyType { get; private set; }
        public TransactionType? TransactionType { get; private set; }

        public string Street { get; private set; } = default!;
        public string City { get; private set; } = default!;
        public string State { get; private set; } = default!;
        public string Country { get; private set; } = default!;

        public decimal MinPrice { get; private set; }
        public decimal MaxPrice { get; private set; }

        public int MinBedrooms { get; private set; }
        public int MaxBedrooms { get; private set; }

        public decimal MinSizeInSqm { get; private set; }
        public decimal MaxSizeInSqm { get; private set; }

        public bool AlertEnabled { get; private set; } = true;

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? LastAlertSentAt { get; private set; }

        public User User { get; private set; } = null!;

        // Private constructor for EF Core
        private SavedSearch() { }

        public static SavedSearch Create(
            UserId userId,
            string name,
            PropertyType propertyType,
            TransactionType? transactionType,
            string country,
            string state,
            string city,
            string street,
            decimal minPrice,
            decimal maxPrice,
            int minBedrooms,
            int maxBedrooms,
            decimal minSizeInSqm,
            decimal maxSizeInSqm)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            if (minPrice > maxPrice)
                throw new ArgumentException("Min price cannot be greater than max price");

            if (minBedrooms > maxBedrooms)
                throw new ArgumentException("Min bedrooms cannot be greater than max bedrooms");

            if (minSizeInSqm > maxSizeInSqm)
                throw new ArgumentException("Min size cannot be greater than max size");

            var savedSearch = new SavedSearch
            {
                Id = SavedSearchId.New(),
                UserId = userId,
                Name = name,
                PropertyType = propertyType,
                TransactionType = transactionType,
                Country = country,
                State = state,
                City = city,
                Street = street,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinBedrooms = minBedrooms,
                MaxBedrooms = maxBedrooms,
                MinSizeInSqm = minSizeInSqm,
                MaxSizeInSqm = maxSizeInSqm,
                AlertEnabled = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Optional domain event
            savedSearch.RaiseDomainEvent(new SavedSearchCreatedEvent(savedSearch.Id));

            return savedSearch;
        }

        public void EnableAlerts()
        {
            AlertEnabled = true;
        }

        public void DisableAlerts()
        {
            AlertEnabled = false;
        }

        public void MarkAlertSent()
        {
            LastAlertSentAt = DateTimeOffset.UtcNow;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            Name = name;
        }
    }
}