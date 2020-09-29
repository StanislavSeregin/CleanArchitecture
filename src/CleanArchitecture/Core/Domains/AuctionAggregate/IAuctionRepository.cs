using System.Threading.Tasks;

namespace Core.Domains.AuctionAggregate
{
    public interface IAuctionRepository
    {
        Task InsertAsync(Auction auction);
    }
}
