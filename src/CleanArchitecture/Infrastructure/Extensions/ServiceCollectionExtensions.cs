using Microsoft.Extensions.DependencyInjection;
using Core.IServices;
using Infrastructure.Services;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
            => services
                .AddScoped<INotificationService, NotificationService>();
    }
}
