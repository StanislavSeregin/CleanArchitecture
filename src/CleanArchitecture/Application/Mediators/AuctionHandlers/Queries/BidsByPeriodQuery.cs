using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Core.Domains.AuctionAggregate;
using Data.AuctionDb;

namespace Application.Mediators.AuctionHandlers.Queries
{
    public class BidsByPeriodQuery : IRequest<Bid[]>
    {
        public DateTimeOffset BidCreatedAtFrom { get; private set; }
        public DateTimeOffset BidCreatedAtTo { get; private set; }

        public BidsByPeriodQuery(
            DateTimeOffset bidCreatedAtFrom,
            DateTimeOffset bidCreatedAtTo
        )
        {
            BidCreatedAtFrom = bidCreatedAtFrom;
            BidCreatedAtTo = bidCreatedAtTo;
        }
    }

    public class BidsByPeriodQueryHandler : IRequestHandler<BidsByPeriodQuery, Bid[]>
    {
        private readonly AuctionDbContext _auctionDbContext;

        public BidsByPeriodQueryHandler(
            AuctionDbContext auctionDbContext
        )
        {
            _auctionDbContext = auctionDbContext;
        }

        public Task<Bid[]> Handle(BidsByPeriodQuery request, CancellationToken cancellationToken)
            => _auctionDbContext.Bids
                .Where(bid => bid.CreatedAt >= request.BidCreatedAtFrom)
                .Where(bid => bid.CreatedAt <= request.BidCreatedAtTo)
                .ToArrayAsync();
    }
}
