using System;
using System.Threading.Tasks;

namespace Core.Domains.AuctionAggregate
{
    public interface IAuctionRepository
    {
        Task<Auction> GetAsync(int auctionId);
        Task InsertAsync(Auction auction);
        Task<Auction> UpdateAsync(int auctionId, Action<Auction> action);
        Task<(Auction auction, T funcResult)> UpdateAsync<T>(int auctionId, Func<Auction, T> func, Predicate<T> persistPredicate = null);
    }
}
