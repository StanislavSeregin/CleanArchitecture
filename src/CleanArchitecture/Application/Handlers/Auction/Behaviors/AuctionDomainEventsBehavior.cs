using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Handlers.Auction.Behaviors
{
    public class AuctionDomainEventsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return next();
        }
    }
}
