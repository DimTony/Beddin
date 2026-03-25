using Beddin.Application.Common.DTOs;
using Beddin.Domain.Aggregates.Properties;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;

namespace Beddin.Application.Common.Interfaces
{
    public interface IRepository<TAggregate, TId>
        where TAggregate : AggregateRoot<TId>
    {
        Task<TAggregate?> GetByIdAsync(TId id, CancellationToken ct = default);
        Task AddAsync(TAggregate aggregate, CancellationToken ct = default);
        Task UpdateAsync(TAggregate aggregate, CancellationToken ct = default);
        Task DeleteAsync(TAggregate aggregate, CancellationToken ct = default);
    }

    public interface IUserRepository : IRepository<User, UserId>
    {
        Task<User?> GetByEmail(string email, CancellationToken ct = default);
        Task<User?> GetUserByRefreshToken(string refreshToken, CancellationToken ct = default);
        Task<PagedResult<User>> GetAllUsers(Guid? userId, string? firstName, string? lastName, string? email, Guid? roleId, int pageNumber, int pageSize, CancellationToken ct = default);

    }

    public interface IRoleRepository : IRepository<Role, RoleId>
    {
        Task<Role?> GetByName(string name, CancellationToken ct = default);
        Task<PagedResult<Role>> GetAllRoles(int pageNumber, int pageSize, int skip, string? name, CancellationToken ct = default);
    }

    public interface IResetPasswordRepository : IRepository<PasswordResetToken, PasswordResetTokenId>
    {
        Task<PasswordResetToken?> GetByToken(string token, CancellationToken ct = default);
        Task<IEnumerable<PasswordResetToken>> GetAllActiveUserTokens(UserId userId, CancellationToken ct = default);
    }

    public interface IUserSessionRepository
    {
        Task<UserSession?> GetById(Guid sessionId, CancellationToken ct = default);
        Task<UserSession?> GetByTokenHash(string tokenHash, CancellationToken ct = default);
        Task<UserSession?> GetActiveSession(UserId userId, CancellationToken ct = default);
        Task<IEnumerable<UserSession>> GetAllActiveSessions(UserId userId, CancellationToken ct = default);
        Task<IEnumerable<UserSession>> GetSessionHistory(UserId userId, CancellationToken ct = default);
        Task<PagedResult<UserSession>> GetSessions(int pageNumber, int pageSize, string? userId, CancellationToken ct = default);
        Task Add(UserSession session, CancellationToken ct = default);
        Task Update(UserSession session, CancellationToken ct = default);
        Task InvalidateAll(UserId userId, string reason, CancellationToken ct = default);
    }

    public interface IPropertyRepository : IRepository<Property, PropertyId>
    {
        //Task<IEnumerable<Property>> GetByOwner(
        //    Guid ownerId,
        //    int pageNumber,
        //    int pageSize,
        //    PropertyStatus? status = null,
        //    CancellationToken ct = default);
        Task<IEnumerable<Property>> GetPropertiesByOwner(UserId ownerId, CancellationToken ct = default);

    }
}
