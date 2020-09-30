using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Core.Domains.AuctionAggregate;
using Application.Mediators.AuctionHandlers.Notifications;

namespace Application.Mediators.AuctionHandlers.Commands
{
    public class ActivateAuctionCommand : IRequest<Auction>
    {
        public int AuctionId { get; private set; }
        public DateTimeOffset AuctionCloseTo { get; private set; }
        public decimal? BiddingStartsWith { get; private set; }
        public decimal? BidStep { get; private set; }
        public decimal? BuyoutPrice { get; private set; }

        public ActivateAuctionCommand(
            int auctionId,
            DateTimeOffset auctionCloseTo,
            decimal? biddingStartsWith = null,
            decimal? bidStep = null,
            decimal? buyoutPrice = null
        )
        {
            AuctionId = auctionId;
            AuctionCloseTo = auctionCloseTo;
            BiddingStartsWith = biddingStartsWith;
            BidStep = bidStep;
            BuyoutPrice = buyoutPrice;
        }
    }

    public class ActivateAuctionCommandHandler : IRequestHandler<ActivateAuctionCommand, Auction>
    {
        private readonly IMediator _mediator;
        private readonly IAuctionRepository _auctionRepository;

        public ActivateAuctionCommandHandler(
            IMediator mediator,
            IAuctionRepository auctionRepository
        )
        {
            _mediator = mediator;
            _auctionRepository = auctionRepository;
        }

        public async Task<Auction> Handle(ActivateAuctionCommand request, CancellationToken cancellationToken)
        {
            var (auction, _) = await _auctionRepository.UpdateAsync(
                request.AuctionId,
                auction => auction
                    .Activate(request.AuctionCloseTo)
                    .ConfigureRules(
                        request.BiddingStartsWith ?? Auction.DefaultBiddingStartsWith,
                        request.BidStep ?? Auction.DefaultBidStep,
                        request.BuyoutPrice
                    )
            );

            await _mediator.Publish(new AuctionPersistedNotification(auction));
            return auction;
        }
    }
}
