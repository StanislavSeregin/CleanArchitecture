using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Core.Domains.AuctionAggregate;
using Core.Domains.AuctionAggregate.Results;
using Application.Mediators.AuctionHandlers.Notifications;

namespace Application.Mediators.AuctionHandlers.Commands
{
    public class MakeBidCommand : IRequest<IBidResult>
    {
        public int AuctionId { get; private set; }
        public Bid Bid { get; private set; }

        public MakeBidCommand(int auctionId, Bid bid)
        {
            AuctionId = auctionId;
            Bid = bid;
        }
    }

    public class MakeBidCommandHandler : IRequestHandler<MakeBidCommand, IBidResult>
    {
        private readonly IMediator _mediator;
        private readonly IAuctionRepository _auctionRepository;

        public MakeBidCommandHandler(
            IMediator mediator,
            IAuctionRepository auctionRepository
        )
        {
            _mediator = mediator;
            _auctionRepository = auctionRepository;
        }

        public async Task<IBidResult> Handle(MakeBidCommand request, CancellationToken cancellationToken)
        {
            static bool IsSuccess(IBidResult bidResult) => bidResult is Ok || bidResult is Buyout;
            var (auction, bidResult) = await _auctionRepository.UpdateAsync(
                request.AuctionId,
                auction => auction.MakeBid(request.Bid),
                IsSuccess
            );

            if (IsSuccess(bidResult))
                await _mediator.Publish(new AuctionPersistedNotification(auction));

            return bidResult;
        }
    }
}
