// <copyright file="DomainEvents.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Domain.Common;

namespace Beddin.Domain.Events
{
#pragma warning disable SA1402
#pragma warning disable SA1649
    /// <summary>
    /// Event raised when an audit log is recorded.
    /// </summary>
    public record AuditLogRecordedEvent(
      AuditLogId AuditLogId,
      string Action,
      string Resource,
      Guid? ResourceId,
      string? OldValue,
      string? NewValue,
      string? IpAddress) : DomainEvent;
#pragma warning restore SA1649

    /// <summary>
    /// Event raised when an audit log is updated.
    /// </summary>
    public record AuditLogUpdatedEvent(
      AuditLogId AuditLogId,
      string Action,
      string Resource,
      Guid? ResourceId,
      string? OldValue,
      string? NewValue,
      string? IpAddress) : DomainEvent;

    /// <summary>
    /// Event raised when a role is created.
    /// </summary>
    public record RoleCreatedEvent(
      RoleId RoleId,
      string Name,
      string Description) : DomainEvent;

    /// <summary>
    /// Event raised when a user is created.
    /// </summary>
    public record UserCreatedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email,
      RoleId Role) : DomainEvent;

    /// <summary>
    /// Event raised when a user's password is changed.
    /// </summary>
    public record PasswordChangedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;

    /// <summary>
    /// Event raised when a password reset token is created.
    /// </summary>
    public record PasswordResetTokenCreatedEvent(
        UserId UserId,
        string Token,
        DateTime ExpiresAt) : DomainEvent;

    /// <summary>
    /// Event raised when a password reset token is used.
    /// </summary>
    public record PasswordResetTokenUsedEvent(
        PasswordResetTokenId Id,
        UserId UserId,
        DateTime UsedAt) : DomainEvent;

    /// <summary>
    /// Event raised when a password reset token is revoked.
    /// </summary>
    public record PasswordResetTokenRevokedEvent(
       PasswordResetTokenId Id,
       UserId UserId,
       DateTime RevokedAt) : DomainEvent;

    /// <summary>
    /// Event raised when a user is locked out.
    /// </summary>
    public record UserLockedOutEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email,
      DateTime LockedOutUntil) : DomainEvent;

    /// <summary>
    /// Event raised when an email confirmation token is generated.
    /// </summary>
    public record EmailConfirmationTokenGeneratedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email,
      string Token) : DomainEvent;

    /// <summary>
    /// Event raised when an email is confirmed.
    /// </summary>
    public record EmailConfirmedEvent(
        UserId UserId,
        string FirstName,
        string LastName,
        string Email) : DomainEvent;

    /// <summary>
    /// Event raised when a user's email is updated.
    /// </summary>
    public record EmailUpdatedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;

    /// <summary>
    /// Event raised when a user's role is updated.
    /// </summary>
    public record RoleUpdatedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email,
      RoleId NewRole) : DomainEvent;

    /// <summary>
    /// Event raised when a user is deactivated.
    /// </summary>
    public record UserDeactivatedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;

    /// <summary>
    /// Event raised when a user is activated.
    /// </summary>
    public record UserActivatedEvent(
      UserId UserId,
      string FirstName,
      string LastName,
      string Email) : DomainEvent;

    /// <summary>
    /// Event raised when a property is created.
    /// </summary>
    public record PropertyCreatedEvent(
      PropertyId PropertyId,
      string Title,
      decimal Price,
      UserId Owner) : DomainEvent;

    /// <summary>
    /// Event raised when a property is published.
    /// </summary>
    public record PropertyPublishedEvent(
      PropertyId PropertyId,
      UserId Owner) : DomainEvent;

    /// <summary>
    /// Event raised when a property is unpublished.
    /// </summary>
    public record PropertyUnpublishedEvent(
      PropertyId PropertyId,
      UserId Owner) : DomainEvent;

    /// <summary>
    /// Event raised when a property's price is updated.
    /// </summary>
    public record PriceUpdatedEvent(
      PropertyId PropertyId,
      UserId Owner,
      decimal NewPrice) : DomainEvent;

    /// <summary>
    /// Event raised when an amenity is added to a property.
    /// </summary>
    public record AmenityAddedEvent(
      PropertyId PropertyId,
      UserId Owner,
      AmenityId AmenityId) : DomainEvent;

    /// <summary>
    /// Event raised when an amenity is removed from a property.
    /// </summary>
    public record AmenityRemovedEvent(
       PropertyId PropertyId,
       UserId Owner,
       AmenityId AmenityId) : DomainEvent;

    /// <summary>
    /// Event raised when amenities are set for a property.
    /// </summary>
    public record AmenitiesSetEvent(
       PropertyId PropertyId,
       UserId Owner,
       IEnumerable<AmenityId> AmenityIds) : DomainEvent;

    /// <summary>
    /// Event raised when a favorite is created.
    /// </summary>
    public record FavoriteCreatedEvent(
      FavoriteId Id,
      UserId UserId,
      PropertyId PropertyId,
      DateTime CreatedAt) : DomainEvent;

    /// <summary>
    /// Event raised when a saved search is created.
    /// </summary>
    public record SavedSearchCreatedEvent(
      SavedSearchId Id) : DomainEvent;
#pragma warning restore SA1402
}
