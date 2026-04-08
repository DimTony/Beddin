// <copyright file="PropertyImage.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System;
using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Properties
{
    /// <summary>
    /// Represents an image associated with a property.
    /// </summary>
    public sealed class PropertyImage : AggregateRoot<PropertyImageId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyImage"/> class.
        /// Private constructor for EF Core.
        /// </summary>
        private PropertyImage()
        {
        }

        /// <summary>
        /// Gets the public identifier for the image (e.g., from cloud storage).
        /// </summary>
        public string PublicId { get; private set; } = default!;

        /// <summary>
        /// Gets the property identifier this image belongs to.
        /// </summary>
        public PropertyId PropertyId { get; private set; } = default!;

        /// <summary>
        /// Gets the URL of the image.
        /// </summary>
        public string ImageUrl { get; private set; } = default!;

        /// <summary>
        /// Gets the URL of the thumbnail image.
        /// </summary>
        public string? ThumbnailUrl { get; private set; }

        /// <summary>
        /// Gets the display order of the image.
        /// </summary>
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is the primary image.
        /// </summary>
        public bool IsPrimary { get; private set; }

        /// <summary>
        /// Gets the date and time when the image was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the property associated with this image.
        /// </summary>
        public Property Property { get; private set; } = null!;

        /// <summary>
        /// Creates a new <see cref="PropertyImage"/> instance.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <param name="publicId">The public identifier.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="thumbnailUrl">The thumbnail URL.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <param name="isPrimary">Whether this is the primary image.</param>
        /// <returns>A new <see cref="PropertyImage"/> instance.</returns>
        public static PropertyImage Create(
            PropertyId propertyId,
            string publicId,
            string imageUrl,
            string? thumbnailUrl,
            int displayOrder,
            bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Image URL is required");
            }

            if (string.IsNullOrWhiteSpace(publicId))
            {
                throw new ArgumentException("PublicId is required");
            }

            if (displayOrder < 0)
            {
                throw new ArgumentException("Display order cannot be negative");
            }

            return new PropertyImage
            {
                Id = PropertyImageId.New(),
                PropertyId = propertyId,
                PublicId = publicId,
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl,
                DisplayOrder = displayOrder,
                IsPrimary = isPrimary,
                CreatedAt = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Sets this image as the primary image.
        /// </summary>
        public void SetAsPrimary()
        {
            this.IsPrimary = true;
        }

        /// <summary>
        /// Removes this image as the primary image.
        /// </summary>
        public void RemoveAsPrimary()
        {
            this.IsPrimary = false;
        }

        /// <summary>
        /// Updates the display order of the image.
        /// </summary>
        /// <param name="newOrder">The new display order.</param>
        public void UpdateOrder(int newOrder)
        {
            if (newOrder < 0)
            {
                throw new ArgumentException("Invalid display order");
            }

            this.DisplayOrder = newOrder;
        }

        /// <summary>
        /// Updates the thumbnail URL of the image.
        /// </summary>
        /// <param name="thumbnailUrl">The new thumbnail URL.</param>
        public void UpdateThumbnail(string? thumbnailUrl)
        {
            this.ThumbnailUrl = thumbnailUrl;
        }
    }
}
