using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Domains.AuctionAggregate;

namespace Data.Auction
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

        public async Task InsertAsync(Core.Domains.AuctionAggregate.Auction auction)
        {
            if (auction == null)
                throw new ArgumentNullException(nameof(auction));

            await _auctionDbContext.AddAsync(auction);
            await _auctionDbContext.SaveChangesAsync();
            _auctionDbContext.Entry(auction).State = EntityState.Detached;
        }
    }
}
