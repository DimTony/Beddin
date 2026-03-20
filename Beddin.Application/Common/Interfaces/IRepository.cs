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

    public interface IUserSessionRepository
    {
        Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken ct = default);
        Task<UserSession?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
        Task<UserSession?> GetActiveSessionAsync(UserId userId, CancellationToken ct = default);
        Task<IEnumerable<UserSession>> GetSessionHistoryAsync(UserId userId, CancellationToken ct = default);
        Task AddAsync(UserSession session, CancellationToken ct = default);
        Task UpdateAsync(UserSession session, CancellationToken ct = default);
        Task InvalidateAllAsync(UserId userId, string reason, CancellationToken ct = default);
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
