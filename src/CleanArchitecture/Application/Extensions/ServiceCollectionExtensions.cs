using Microsoft.Extensions.DependencyInjection;
using MediatR;
using MediatR.Pipeline;
using Application.Handlers.Auction.Behaviors;
using Core.IServices;
using Application.Services;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(ServiceCollectionExtensions).Assembly;
            return services
                .AddMediatR(assembly)
                .AddScoped(typeof(IRequestPostProcessor<,>), typeof(AuctionDomainEventsPostProcessor<,>))
                .AddScoped<IAuctionNotificationService, AuctionNotificationService>();
        }
    }
}
