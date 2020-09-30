using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domains.AuctionAggregate.Events;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Handlers.Auction.Behaviors
{
    public class AuctionDomainEventsPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IAuctionCommand
        where TResponse : Core.Domains.AuctionAggregate.Auction
    {
        private readonly ILogger<AuctionDomainEventsPostProcessor<TRequest, TResponse>> _logger;

        public AuctionDomainEventsPostProcessor(
            ILogger<AuctionDomainEventsPostProcessor<TRequest, TResponse>> logger
        )
        {
            _logger = logger;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            var domainEvents = response.GetDomainEvents();
            return Task.WhenAll(domainEvents.Select(HandleDomainEventAsync));
        }

        private async Task HandleDomainEventAsync(IAuctionDomainEvent domainEvent)
        {
            Task task;
            try
            {
                task = domainEvent switch
                {
                    Created _ => Task.CompletedTask,
                    Activated _ => Task.CompletedTask,
                    NewBid _ => Task.CompletedTask,
                    Buyouted _ => Task.CompletedTask,
                    Closed _ => Task.CompletedTask,
                    _ => ActionAsTask(() => _logger.LogError($"Unexpected domain event type: '{domainEvent.GetType().Name}'"))
                };

                await task;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error handling domain event: '{domainEvent.GetType().Name}'", e);
            }
        }

        private Task ActionAsTask(Action action)
        {
            action();
            return Task.CompletedTask;
        }
    }
}
