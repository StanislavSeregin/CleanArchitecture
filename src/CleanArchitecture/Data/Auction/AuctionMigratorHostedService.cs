using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Data.Auction
{
    public class AuctionMigratorHostedService : IHostedService
    {
        private readonly AuctionDbContext _auctionDbContext;
        private readonly ILogger<AuctionMigratorHostedService> _logger;

        public AuctionMigratorHostedService(
            AuctionDbContext auctionDbContext,
            ILogger<AuctionMigratorHostedService> logger
        )
        {
            _auctionDbContext = auctionDbContext;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _auctionDbContext.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Applying '{nameof(AuctionDbContext)}' migrations has been failed", e);
            }

            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
