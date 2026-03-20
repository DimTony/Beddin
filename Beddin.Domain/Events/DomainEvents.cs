using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Events
{
    public record PropertyCreatedEvent(
      PropertyId PropertyId,
      string Title,
      decimal Price,
      UserId Owner) : DomainEvent;
    public record PropertyPublishedEvent(
      PropertyId PropertyId,
      UserId Owner) : DomainEvent;
    public record PropertyUnpublishedEvent(
      PropertyId PropertyId,
      UserId Owner) : DomainEvent;
    public record PriceUpdatedEvent(
      PropertyId PropertyId,
      UserId Owner,
      decimal NewPrice) : DomainEvent;
    public record AmenityAddedEvent(
      PropertyId PropertyId,
      UserId Owner,
      AmenityId AmenityId) : DomainEvent;
    public record AmenityRemovedEvent(
       PropertyId PropertyId,
       UserId Owner,
       AmenityId AmenityId) : DomainEvent;
    public record AmenitiesSetEvent(
       PropertyId PropertyId,
       UserId Owner,
       IEnumerable<AmenityId> AmenityIds) : DomainEvent;
    public record FavoriteCreatedEvent(
      FavoriteId Id,
      UserId UserId,
      PropertyId PropertyId,
      DateTime CreatedAt) : DomainEvent;

    


}
