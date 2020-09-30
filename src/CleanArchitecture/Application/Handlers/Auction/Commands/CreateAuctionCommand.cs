using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Core.Domains.AuctionAggregate;
using Application.Handlers.Auction.Notifications;

namespace Application.Handlers.Auction.Commands
{
    public class CreateAuctionCommand : IRequest<Core.Domains.AuctionAggregate.Auction>
    {
        public Lot Lot { get; private set; }

        public CreateAuctionCommand(Lot lot)
        {
            Lot = lot ?? throw new ArgumentNullException(nameof(lot));
        }
    }

    public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Core.Domains.AuctionAggregate.Auction>
    {
        private readonly IMediator _mediator;
        private readonly IAuctionRepository _auctionRepository;

        public CreateAuctionCommandHandler(
            IMediator mediator,
            IAuctionRepository auctionRepository
        )
        {
            _mediator = mediator;
            _auctionRepository = auctionRepository;
        }

        public async Task<Core.Domains.AuctionAggregate.Auction> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            var auction = new Core.Domains.AuctionAggregate.Auction(request.Lot);
            await _auctionRepository.InsertAsync(auction);
            await _mediator.Publish(new AuctionPersistedNotification(auction));
            return auction;
        }
    }
}
