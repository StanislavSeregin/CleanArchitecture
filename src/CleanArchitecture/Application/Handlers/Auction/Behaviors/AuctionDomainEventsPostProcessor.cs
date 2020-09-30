using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR.Pipeline;
using Core.Domains.AuctionAggregate.Events;
using Core.IServices;

namespace Application.Handlers.Auction.Behaviors
{
    public class AuctionDomainEventsPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IAuctionCommand
        where TResponse : Core.Domains.AuctionAggregate.Auction
    {
        private readonly ILogger<AuctionDomainEventsPostProcessor<TRequest, TResponse>> _logger;
        private readonly IAuctionNotificationService _auctionNotificationService;

        public AuctionDomainEventsPostProcessor(
            ILogger<AuctionDomainEventsPostProcessor<TRequest, TResponse>> logger,
            IAuctionNotificationService auctionNotificationService
        )
        {
            _logger = logger;
            _auctionNotificationService = auctionNotificationService;
        }

        public Task Process(TRequest request, TResponse auction, CancellationToken cancellationToken)
        {
            var domainEvents = auction.GetDomainEvents();
            var tasks = domainEvents.Select(domainEvent => HandleDomainEventAsync(domainEvent, auction));
            return Task.WhenAll(tasks);
        }

        private async Task HandleDomainEventAsync(IAuctionDomainEvent domainEvent, Core.Domains.AuctionAggregate.Auction auction)
        {
            Task task;
            try
            {
                task = domainEvent switch
                {
                    Activated _ => _auctionNotificationService.NotifyAboutActivationAsync(auction),
                    NewBid _ => _auctionNotificationService.NotifyAboutNewBidAsync(auction),
                    Buyouted _ => _auctionNotificationService.NotifyAboutBuyoutAsync(auction),
                    Closed _ => _auctionNotificationService.NotifyAboutCloseAsync(auction),
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
