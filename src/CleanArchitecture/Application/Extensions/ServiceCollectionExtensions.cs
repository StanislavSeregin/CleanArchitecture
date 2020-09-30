using Microsoft.Extensions.DependencyInjection;
using MediatR;
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
                .AddScoped<IAuctionNotificationService, AuctionNotificationService>();
        }
    }
}
