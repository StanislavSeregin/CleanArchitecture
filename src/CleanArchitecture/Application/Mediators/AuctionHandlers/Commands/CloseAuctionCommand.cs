using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Core.Domains.AuctionAggregate;
using Application.Mediators.AuctionHandlers.Notifications;

namespace Application.Mediators.AuctionHandlers.Commands
{
    public class CloseAuctionCommand : IRequest<Auction>
    {
        public int AuctionId { get; private set; }

        public CloseAuctionCommand(int auctionId)
        {
            AuctionId = auctionId;
        }
    }

    public class CloseAuctionCommandHandler : IRequestHandler<CloseAuctionCommand, Auction>
    {
        private readonly IMediator _mediator;
        private readonly IAuctionRepository _auctionRepository;

        public CloseAuctionCommandHandler(
            IMediator mediator,
            IAuctionRepository auctionRepository
        )
        {
            _mediator = mediator;
            _auctionRepository = auctionRepository;
        }

        public async Task<Auction> Handle(CloseAuctionCommand request, CancellationToken cancellationToken)
        {
            var (auction, _) = await _auctionRepository.UpdateAsync(
                request.AuctionId,
                auction => auction.Close()
            );

            await _mediator.Publish(new AuctionPersistedNotification(auction));
            return auction;
        }
    }
}
