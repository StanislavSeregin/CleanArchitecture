using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using Core.Domains.AuctionAggregate.Events;
using Core.IServices;
using Core.Domains.AuctionAggregate;

namespace Application.Mediators.AuctionHandlers.Notifications
{
    public class AuctionPersistedNotification : INotification
    {
        public Auction Auction { get; private set; }

        public AuctionPersistedNotification(Auction auction)
        {
            Auction = auction;
        }
    }

    public class AuctionPersistedNotificationHandler : INotificationHandler<AuctionPersistedNotification>
    {
        private readonly ILogger<AuctionPersistedNotificationHandler> _logger;
        private readonly IAuctionNotificationService _auctionNotificationService;

        public AuctionPersistedNotificationHandler(
            ILogger<AuctionPersistedNotificationHandler> logger,
            IAuctionNotificationService auctionNotificationService
        )
        {
            _logger = logger;
            _auctionNotificationService = auctionNotificationService;
        }

        public async Task Handle(AuctionPersistedNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var (domainEvents, auction) = (notification.Auction.GetDomainEvents(), notification.Auction);
                var tasks = domainEvents.Select(domainEvent => HandleDomainEventAsync(domainEvent, auction));
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError("Error handle auction domain events", e);
            }
        }

        private async Task HandleDomainEventAsync(IAuctionDomainEvent domainEvent, Auction auction)
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
                    _ => ActionAsTask(() => _logger.LogError($"Unexpected auction domain event type: '{domainEvent.GetType().Name}'"))
                };

                await task;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error handle auction domain event: '{domainEvent.GetType().Name}'", e);
            }
        }

        private Task ActionAsTask(Action action)
        {
            action();
            return Task.CompletedTask;
        }
    }
}
