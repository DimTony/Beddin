using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Common.Behaviours
{
    public class DomainEventBehavior<TRequest, TResponse>
       : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        private readonly IMediator _mediator;
        private readonly IDomainEventCollector _collector;
        private readonly IUnitOfWork _uow;

        public DomainEventBehavior(
            IMediator mediator,
            IDomainEventCollector collector,
            IUnitOfWork uow)
        {
            _mediator = mediator;
            _collector = collector;
            _uow = uow;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            // Step 1: Run the handler — aggregates mutate but nothing is saved yet
            var response = await next();

            // Step 2: Commit the primary aggregate state
            await _uow.SaveChangesAsync(ct);

            // Steps 3-6: Dispatch events and save side-effects, up to 5 rounds
            // (prevents infinite loops from event chains while supporting 2-level chains)
            const int maxRounds = 5;
            for (int round = 0; round < maxRounds; round++)
            {
                var events = _collector.CollectAndClear().ToList();
                if (events.Count == 0) break;

                foreach (var domainEvent in events)
                    await _mediator.Publish(domainEvent, ct);

                // Persist anything the notification handlers added (e.g. SavingsAccount)
                await _uow.SaveChangesAsync(ct);
            }

            return response;
        }
    }
}
