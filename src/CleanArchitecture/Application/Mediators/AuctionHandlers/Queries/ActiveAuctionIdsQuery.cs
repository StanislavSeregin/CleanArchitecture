using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Core.Domains.AuctionAggregate;
using Data.AuctionDb;

namespace Application.Mediators.AuctionHandlers.Queries
{
    public class ActiveAuctionIdsQuery : IRequest<int[]> { }

    public class ActiveAuctionIdsQueryHandler : IRequestHandler<ActiveAuctionIdsQuery, int[]>
    {
        private readonly AuctionDbContext _auctionDbContext;

        public ActiveAuctionIdsQueryHandler(
            AuctionDbContext auctionDbContext
        )
        {
            _auctionDbContext = auctionDbContext;
        }

        public Task<int[]> Handle(ActiveAuctionIdsQuery request, CancellationToken cancellationToken)
            => _auctionDbContext.Auctions
                .AsNoTracking()
                .Where(auction => auction.Status == Status.Active)
                .Select(auction => auction.Id)
                .ToArrayAsync();
    }
}
