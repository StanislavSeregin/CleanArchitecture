using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Domains.AuctionAggregate;

namespace Data.AuctionDb
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AuctionDbContext _auctionDbContext;

        public AuctionRepository(
            AuctionDbContext auctionDbContext
        )
        {
            _auctionDbContext = auctionDbContext;
        }

        public async Task<Auction> GetAsync(int auctionId)
        {
            var aggregate = await GetAggregateAsync(auctionId, true);
            _auctionDbContext.Entry(aggregate).State = EntityState.Detached;
            return aggregate;
        }

        public async Task InsertAsync(Auction auction)
        {
            if (auction == null)
                throw new ArgumentNullException(nameof(auction));

            await _auctionDbContext.AddAsync(auction);
            await _auctionDbContext.SaveChangesAsync();
            _auctionDbContext.Entry(auction).State = EntityState.Detached;
        }

        public async Task<Auction> UpdateAsync(int auctionId, Action<Auction> action)
        {
            var aggregate = await GetAggregateAsync(auctionId);
            action(aggregate);
            await _auctionDbContext.SaveChangesAsync();
            _auctionDbContext.Entry(aggregate).State = EntityState.Detached;
            return aggregate;
        }

        public async Task<(Auction auction, T funcResult)> UpdateAsync<T>(int auctionId, Func<Auction, T> func, Predicate<T> persistPredicate = null)
        {
            var aggregate = await GetAggregateAsync(auctionId);
            var funcResult = func(aggregate);
            if (persistPredicate == null || persistPredicate(funcResult))
            {
                await _auctionDbContext.SaveChangesAsync();
                _auctionDbContext.Entry(aggregate).State = EntityState.Detached;
            }

            return (aggregate, funcResult);
        }

        private Task<Auction> GetAggregateAsync(int id, bool asNoTracking = false)
        {
            var query = asNoTracking
                ? _auctionDbContext.Auctions.AsNoTracking()
                : _auctionDbContext.Auctions;

            return query
                .Include(x => x.Lot)
                .Include(x => x.Bids)
                .FirstOrDefaultAsync(auction => auction.Id == id);
        }
    }
}
