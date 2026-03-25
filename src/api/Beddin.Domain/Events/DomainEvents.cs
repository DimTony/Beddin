using Beddin.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Domain.Events
{

    
        public record AuditLogRecordedEvent(
      AuditLogId AuditLogId,
      string Action,
      string Resource,
      Guid? ResourceId,
      string? OldValue,
      string? NewValue,
      string? IpAddress) : DomainEvent;
    public record AuditLogUpdatedEvent(
      AuditLogId AuditLogId,
      string Action,
      string Resource,
      Guid? ResourceId,
      string? OldValue,
      string? NewValue,
      string? IpAddress) : DomainEvent;
    public record RoleCreatedEvent(
      RoleId RoleId,
      string Name,
      string Description) : DomainEvent;

    public record UserCreatedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email,
      RoleId Role) : DomainEvent;

    public record PasswordChangedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;

    public record PasswordResetTokenCreatedEvent(
        UserId UserId,
        string Token,
        DateTime ExpiresAt) : DomainEvent;

    public record PasswordResetTokenUsedEvent(
        PasswordResetTokenId Id,
        UserId UserId,
        DateTime UsedAt) : DomainEvent;

    public record PasswordResetTokenRevokedEvent(
       PasswordResetTokenId Id,
       UserId UserId,
       DateTime RevokedAt) : DomainEvent;

    


    public record EmailConfirmationTokenGeneratedEvent(
    UserId UserId,
    string FirstName,
    string LastName,
    string Email,
    string Token) : DomainEvent;

    public record EmailConfirmedEvent(
        UserId UserId,
        string FirstName,
        string LastName,
        string Email) : DomainEvent;

    public record EmailUpdatedEvent(
     UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;

    public record RoleUpdatedEvent(
     UserId UserId,
      string FirstName,
      string LastName,
      string Email,
     RoleId NewRole) : DomainEvent;

    public record UserDeactivatedEvent(
     UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;

    public record UserActivatedEvent(
     UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;


    

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

    public record SavedSearchCreatedEvent(
      SavedSearchId Id) : DomainEvent;
    

}
