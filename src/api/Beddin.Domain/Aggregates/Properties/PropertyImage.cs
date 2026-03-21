using Beddin.Domain.Common;

namespace Beddin.Domain.Aggregates.Properties
{
    public sealed class PropertyImage : AggregateRoot<PropertyImageId>
    {
        public string PublicId { get; private set; } = default!;
        public PropertyId PropertyId { get; private set; } = default!;
        public string ImageUrl { get; private set; } = default!;
        public string? ThumbnailUrl { get; private set; }

        public int DisplayOrder { get; private set; }
        public bool IsPrimary { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public Property Property { get; private set; } = null!;

        private PropertyImage() { }

        public static PropertyImage Create(
            PropertyId propertyId,
            string publicId,
            string imageUrl,
            string? thumbnailUrl,
            int displayOrder,
            bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL is required");

            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("PublicId is required");

            if (displayOrder < 0)
                throw new ArgumentException("Display order cannot be negative");

            return new PropertyImage
            {
                Id = PropertyImageId.New(),
                PropertyId = propertyId,
                PublicId = publicId,
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl,
                DisplayOrder = displayOrder,
                IsPrimary = isPrimary,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void SetAsPrimary()
        {
            IsPrimary = true;
        }

        public void RemoveAsPrimary()
        {
            IsPrimary = false;
        }

        public void UpdateOrder(int newOrder)
        {
            if (newOrder < 0)
                throw new ArgumentException("Invalid display order");

            DisplayOrder = newOrder;
        }

        public void UpdateThumbnail(string? thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
        }
    }
}