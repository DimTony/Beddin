// <copyright file="DomainEventBehavior.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;
using MediatR;

namespace Beddin.Application.Common.Behaviours
{
    /// <summary>
    /// Pipeline behavior that handles publishing domain events after a request is processed.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    public class DomainEventBehavior<TRequest, TResponse>
       : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        private readonly IMediator mediator;
        private readonly IDomainEventCollector collector;
        private readonly IUnitOfWork uow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance used to publish domain events.</param>
        /// <param name="collector">The domain event collector instance.</param>
        /// <param name="uow">The unit of work instance.</param>
        public DomainEventBehavior(
            IMediator mediator,
            IDomainEventCollector collector,
            IUnitOfWork uow)
        {
            this.mediator = mediator;
            this.collector = collector;
            this.uow = uow;
        }

        /// <summary>
        /// Handles the pipeline behavior for publishing domain events and saving changes.
        /// </summary>
        /// <param name="request">The request being handled.</param>
        /// <param name="next">The next delegate in the pipeline.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>The response from the next handler in the pipeline.</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var response = await next();

            await this.uow.SaveChangesAsync(ct);

            const int maxRounds = 5;
            for (int round = 0; round < maxRounds; round++)
            {
                var events = this.collector.CollectAndClear().ToList();
                if (events.Count == 0)
                {
                    break;
                }

                foreach (var domainEvent in events)
                {
                    await this.mediator.Publish(domainEvent, ct);
                }

                await this.uow.SaveChangesAsync(ct);
            }

            return response;
        }
    }
}
