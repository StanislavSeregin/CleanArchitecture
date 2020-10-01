using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Core.Domains.AuctionAggregate;

namespace Application.Mediators.AuctionHandlers.Queries
{
    public class AuctionQuery : IRequest<Auction>
    {
        public int AuctionId { get; private set; }

        public AuctionQuery(int auctionId)
        {
            AuctionId = auctionId;
        }
    }

    public class AuctionQueryHandler : IRequestHandler<AuctionQuery, Auction>
    {
        private readonly IAuctionRepository _auctionRepository;

        public AuctionQueryHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public Task<Auction> Handle(AuctionQuery request, CancellationToken cancellationToken)
            => _auctionRepository.GetAsync(request.AuctionId);
    }
}
