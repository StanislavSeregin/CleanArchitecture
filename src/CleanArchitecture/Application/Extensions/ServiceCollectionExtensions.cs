using Application.Handlers.Auction.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(ServiceCollectionExtensions).Assembly;
            return services
                .AddMediatR(assembly)
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(AuctionDomainEventsBehavior<,>));
        }
    }
}
