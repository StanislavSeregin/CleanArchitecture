using System.Threading.Tasks;
using Core.Domains.AuctionAggregate;
using Core.IServices;

namespace Application.Services
{
    public class AuctionNotificationService : IAuctionNotificationService
    {
        private readonly INotificationService _notificationService;

        public AuctionNotificationService(
            INotificationService notificationService
        )
        {
            _notificationService = notificationService;
        }

        public Task NotifyAboutActivationAsync(Auction auction)
            => Task.CompletedTask;

        public Task NotifyAboutNewBidAsync(Auction auction)
            => Task.CompletedTask;

        public Task NotifyAboutBuyoutAsync(Auction auction)
            => Task.CompletedTask;

        public Task NotifyAboutCloseAsync(Auction auction)
            => Task.CompletedTask;
    }
}
