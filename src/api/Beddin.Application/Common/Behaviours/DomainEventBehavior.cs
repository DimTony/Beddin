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
            var response = await next();

            await _uow.SaveChangesAsync(ct);

            const int maxRounds = 5;
            for (int round = 0; round < maxRounds; round++)
            {
                var events = _collector.CollectAndClear().ToList();
                if (events.Count == 0) break;

                foreach (var domainEvent in events)
                    await _mediator.Publish(domainEvent, ct);

                await _uow.SaveChangesAsync(ct);
            }

            return response;
        }
    }
}
