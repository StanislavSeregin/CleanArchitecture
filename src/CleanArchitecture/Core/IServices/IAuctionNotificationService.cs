using System.Threading.Tasks;
using Core.Domains.AuctionAggregate;

namespace Core.IServices
{
    public interface IAuctionNotificationService
    {
        Task NotifyAboutActivationAsync(Auction auction);
        Task NotifyAboutNewBidAsync(Auction auction);
        Task NotifyAboutBuyoutAsync(Auction auction);
        Task NotifyAboutCloseAsync(Auction auction);
    }
}
