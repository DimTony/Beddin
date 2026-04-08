// <copyright file="IRepository.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.DTOs;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;

#pragma warning disable SA1402 // File may only contain a single type
namespace Beddin.Application.Common.Interfaces
{
    /// <summary>
    /// Generic repository interface for aggregate roots.
    /// </summary>
    /// <typeparam name="TAggregate">The aggregate root type.</typeparam>
    /// <typeparam name="TId">The identifier type.</typeparam>
    public interface IRepository<TAggregate, TId>
        where TAggregate : AggregateRoot<TId>
    {
        /// <summary>
        /// Gets an aggregate by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the aggregate if found, null otherwise.</returns>
        Task<TAggregate?> GetByIdAsync(TId id, CancellationToken ct = default);

        /// <summary>
        /// Adds a new aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate to add.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(TAggregate aggregate, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate to update.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(TAggregate aggregate, CancellationToken ct = default);

        /// <summary>
        /// Deletes an aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(TAggregate aggregate, CancellationToken ct = default);
    }

    /// <summary>
    /// Repository interface for user aggregates.
    /// </summary>
    public interface IUserRepository : IRepository<User, UserId>
    {
        /// <summary>
        /// Gets a user by email address.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the user if found, null otherwise.</returns>
        Task<User?> GetByEmail(string email, CancellationToken ct = default);

        /// <summary>
        /// Gets a user by refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the user if found, null otherwise.</returns>
        Task<User?> GetUserByRefreshToken(string refreshToken, CancellationToken ct = default);

        /// <summary>
        /// Gets all users with pagination and filtering.
        /// </summary>
        /// <param name="userId">The user identifier filter.</param>
        /// <param name="firstName">The first name filter.</param>
        /// <param name="lastName">The last name filter.</param>
        /// <param name="email">The email filter.</param>
        /// <param name="roleId">The role identifier filter.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns a paged result of users.</returns>
        Task<PagedResult<User>> GetAllUsers(Guid? userId, string? firstName, string? lastName, string? email, Guid? roleId, int pageNumber, int pageSize, CancellationToken ct = default);
    }

    /// <summary>
    /// Repository interface for role aggregates.
    /// </summary>
    public interface IRoleRepository : IRepository<Role, RoleId>
    {
        /// <summary>
        /// Gets a role by name.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the role if found, null otherwise.</returns>
        Task<Role?> GetByName(string name, CancellationToken ct = default);

        /// <summary>
        /// Gets all roles with pagination and filtering.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="skip">Number of records to skip.</param>
        /// <param name="name">The name filter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns a paged result of roles.</returns>
        Task<PagedResult<Role>> GetAllRoles(int pageNumber, int pageSize, int skip, string? name, CancellationToken ct = default);
    }

    /// <summary>
    /// Repository interface for password reset token aggregates.
    /// </summary>
    public interface IResetPasswordRepository : IRepository<PasswordResetToken, PasswordResetTokenId>
    {
        /// <summary>
        /// Gets a password reset token by token string.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the password reset token if found, null otherwise.</returns>
        Task<PasswordResetToken?> GetByToken(string token, CancellationToken ct = default);

        /// <summary>
        /// Gets all active password reset tokens for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns a collection of active password reset tokens.</returns>
        Task<IEnumerable<PasswordResetToken>> GetAllActiveUserTokens(UserId userId, CancellationToken ct = default);
    }

    /// <summary>
    /// Repository interface for user session aggregates.
    /// </summary>
    public interface IUserSessionRepository
    {
        /// <summary>
        /// Gets a session by identifier.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the session if found, null otherwise.</returns>
        Task<UserSession?> GetById(Guid sessionId, CancellationToken ct = default);

        /// <summary>
        /// Gets a session by token hash.
        /// </summary>
        /// <param name="tokenHash">The token hash.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the session if found, null otherwise.</returns>
        Task<UserSession?> GetByTokenHash(string tokenHash, CancellationToken ct = default);

        /// <summary>
        /// Gets the active session for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the active session if found, null otherwise.</returns>
        Task<UserSession?> GetActiveSession(UserId userId, CancellationToken ct = default);

        /// <summary>
        /// Gets all active sessions for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns a collection of active sessions.</returns>
        Task<IEnumerable<UserSession>> GetAllActiveSessions(UserId userId, CancellationToken ct = default);

        /// <summary>
        /// Gets the session history for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns a collection of sessions.</returns>
        Task<IEnumerable<UserSession>> GetSessionHistory(UserId userId, CancellationToken ct = default);

        /// <summary>
        /// Gets sessions with pagination and filtering.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="userId">The user identifier filter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns a paged result of sessions.</returns>
        Task<PagedResult<UserSession>> GetSessions(int pageNumber, int pageSize, string? userId, CancellationToken ct = default);

        /// <summary>
        /// Adds a new session.
        /// </summary>
        /// <param name="session">The session to add.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Add(UserSession session, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing session.
        /// </summary>
        /// <param name="session">The session to update.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Update(UserSession session, CancellationToken ct = default);

        /// <summary>
        /// Invalidates all sessions for a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="reason">The reason for invalidation.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InvalidateAll(UserId userId, string reason, CancellationToken ct = default);
    }

    /// <summary>
    /// Repository interface for property aggregates.
    /// </summary>
    public interface IPropertyRepository : IRepository<Property, PropertyId>
    {
        /// <summary>
        /// Gets all properties owned by a specific owner.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns a collection of properties.</returns>
        Task<IEnumerable<Property>> GetPropertiesByOwner(UserId ownerId, CancellationToken ct = default);
    }
}
#pragma warning restore SA1402 // File may only contain a single type
