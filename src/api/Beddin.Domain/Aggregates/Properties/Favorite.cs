// <copyright file="Favorite.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.Properties
{
    /// <summary>
    /// Represents a user's favorite property in the real estate application.
    /// </summary>
    public sealed class Favorite : AggregateRoot<FavoriteId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Favorite"/> class.
        /// </summary>
        public Favorite()
        {
        }

        /// <summary>
        /// Gets the identifier of the user who favorited the property.
        /// </summary>
        public UserId UserId { get; private set; } = null!;

        /// <summary>
        /// Gets the identifier of the property that was favorited.
        /// </summary>
        public PropertyId PropertyId { get; private set; } = null!;

        /// <summary>
        /// Gets the date and time when the favorite was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Favorite"/> instance for the specified user and property.
        /// </summary>
        /// <param name="userId">The identifier of the user who is favoriting the property.</param>
        /// <param name="propertyId">The identifier of the property to be favorited.</param>
        /// <returns>A new <see cref="Favorite"/> instance.</returns>
        public static Favorite Create(UserId userId, PropertyId propertyId)
        {
            var favorite = new Favorite
            {
                Id = FavoriteId.New(),
                UserId = userId,
                PropertyId = propertyId,
                CreatedAt = DateTime.UtcNow,
            };

            favorite.RaiseDomainEvent(new FavoriteCreatedEvent(
              favorite.Id,
              favorite.UserId,
              favorite.PropertyId,
              favorite.CreatedAt));

            return favorite;
        }
    }
}
