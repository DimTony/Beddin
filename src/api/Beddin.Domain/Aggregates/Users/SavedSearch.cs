// <copyright file="SavedSearch.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.Users
{
    /// <summary>
    /// Represents a saved search for properties.
    /// </summary>
    public sealed class SavedSearch : AggregateRoot<SavedSearchId>
    {
        private SavedSearch()
        {
        }

        /// <summary>
        /// Gets the user identifier who created this saved search.
        /// </summary>
        public UserId UserId { get; private set; } = null!;

        /// <summary>
        /// Gets the name of the saved search.
        /// </summary>
        public string Name { get; private set; } = null!;

        /// <summary>
        /// Gets the property type filter.
        /// </summary>
        public PropertyType PropertyType { get; private set; }

        /// <summary>
        /// Gets the transaction type filter.
        /// </summary>
        public TransactionType? TransactionType { get; private set; }

        /// <summary>
        /// Gets the street filter.
        /// </summary>
        public string Street { get; private set; } = default!;

        /// <summary>
        /// Gets the city filter.
        /// </summary>
        public string City { get; private set; } = default!;

        /// <summary>
        /// Gets the state filter.
        /// </summary>
        public string State { get; private set; } = default!;

        /// <summary>
        /// Gets the country filter.
        /// </summary>
        public string Country { get; private set; } = default!;

        /// <summary>
        /// Gets the minimum price filter.
        /// </summary>
        public decimal MinPrice { get; private set; }

        /// <summary>
        /// Gets the maximum price filter.
        /// </summary>
        public decimal MaxPrice { get; private set; }

        /// <summary>
        /// Gets the minimum bedrooms filter.
        /// </summary>
        public int MinBedrooms { get; private set; }

        /// <summary>
        /// Gets the maximum bedrooms filter.
        /// </summary>
        public int MaxBedrooms { get; private set; }

        /// <summary>
        /// Gets the minimum size in square meters filter.
        /// </summary>
        public decimal MinSizeInSqm { get; private set; }

        /// <summary>
        /// Gets the maximum size in square meters filter.
        /// </summary>
        public decimal MaxSizeInSqm { get; private set; }

        /// <summary>
        /// Gets a value indicating whether alerts are enabled for this saved search.
        /// </summary>
        public bool AlertEnabled { get; private set; } = true;

        /// <summary>
        /// Gets the date and time when the saved search was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the last alert was sent.
        /// </summary>
        public DateTimeOffset? LastAlertSentAt { get; private set; }

        /// <summary>
        /// Gets the user who created this saved search.
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearch"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        /// <returns>A new instance of the <see cref="SavedSearch"/> class.</returns>

        /// <summary>
        /// Creates a new <see cref="SavedSearch"/> instance.
        /// </summary>
        /// <param name="userId">The user identifier who created this saved search.</param>
        /// <param name="name">The name of the saved search.</param>
        /// <param name="propertyType">The property type filter.</param>
        /// <param name="transactionType">The transaction type filter.</param>
        /// <param name="country">The country filter.</param>
        /// <param name="state">The state filter.</param>
        /// <param name="city">The city filter.</param>
        /// <param name="street">The street filter.</param>
        /// <param name="minPrice">The minimum price filter.</param>
        /// <param name="maxPrice">The maximum price filter.</param>
        /// <param name="minBedrooms">The minimum bedrooms filter.</param>
        /// <param name="maxBedrooms">The maximum bedrooms filter.</param>
        /// <param name="minSizeInSqm">The minimum size in square meters filter.</param>
        /// <param name="maxSizeInSqm">The maximum size in square meters filter.</param>
        /// <returns>A new <see cref="SavedSearch"/> instance.</returns>
#pragma warning disable SA1611 // Element parameters should be documented
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
#pragma warning restore SA1611 // Element parameters should be documented
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required", nameof(name));
            }

            if (minPrice > maxPrice)
            {
                throw new ArgumentException("Min price cannot be greater than max price");
            }

            if (minBedrooms > maxBedrooms)
            {
                throw new ArgumentException("Min bedrooms cannot be greater than max bedrooms");
            }

            if (minSizeInSqm > maxSizeInSqm)
            {
                throw new ArgumentException("Min size cannot be greater than max size");
            }

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
                CreatedAt = DateTimeOffset.UtcNow,
            };

            savedSearch.RaiseDomainEvent(new SavedSearchCreatedEvent(savedSearch.Id));

            return savedSearch;
        }

        /// <summary>
        /// Enables alerts for this saved search.
        /// </summary>
        public void EnableAlerts()
        {
            this.AlertEnabled = true;
        }

        /// <summary>
        /// Disables alerts for this saved search.
        /// </summary>
        public void DisableAlerts()
        {
            this.AlertEnabled = false;
        }

        /// <summary>
        /// Marks that an alert was sent for this saved search.
        /// </summary>
        public void MarkAlertSent()
        {
            this.LastAlertSentAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Updates the name of the saved search.
        /// </summary>
        /// <param name="name">The new name.</param>
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required", nameof(name));
            }

            this.Name = name;
        }
    }
}
