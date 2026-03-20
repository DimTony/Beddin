using Beddin.Domain.Common;
using Beddin.Domain.Events;

namespace Beddin.Domain.Aggregates.Properties
{
    public class Favorite : AggregateRoot<FavoriteId>
    {
        public UserId UserId { get; private set; } = null!;
        public PropertyId PropertyId { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public Favorite() { }

        public static Favorite Create(UserId userId, PropertyId propertyId)
        {

            var favorite = new Favorite
            {
                Id = FavoriteId.New(),
                UserId = userId,
                PropertyId = propertyId,
                CreatedAt = DateTime.UtcNow
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
