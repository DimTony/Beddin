using Beddin.Domain.Common;

namespace Beddin.Application.Common.Interfaces
{
    public interface IDomainEventCollector
    {
        IEnumerable<DomainEvent> CollectAndClear();
    }
}
