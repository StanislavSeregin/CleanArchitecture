using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Data.AuctionDb;
using Core.Domains.AuctionAggregate;

namespace Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuctionDb(this IServiceCollection services, string connectionString)
            => services
                .AddDbContext<AuctionDbContext>(options => options.UseNpgsql(
                    connectionString,
                    builder => builder.MigrationsAssembly(typeof(AuctionDbContext).Assembly.FullName)
                ))
                .AddAuctionDbRepositories();

        public static IServiceCollection AddInMemoryAuctionDb(this IServiceCollection services)
            => services
                .AddDbContext<AuctionDbContext>(options => options.UseInMemoryDatabase(nameof(AuctionDbContext)))
                .AddAuctionDbRepositories();

        private static IServiceCollection AddAuctionDbRepositories(this IServiceCollection services)
            => services
                .AddScoped<IAuctionRepository, AuctionRepository>();

        public static IServiceCollection AddAuctionDbMigrator(this IServiceCollection services)
            => services.AddHostedService<AuctionMigratorHostedService>();
    }
}

