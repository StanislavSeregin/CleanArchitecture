using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using MediatR;
using Core.Domains.AuctionAggregate;
using Data.Extensions;
using Application.Extensions;
using Application.Handlers.Auction.Commands;

namespace Tests
{
    public class AuctionTests
    {
        private readonly IMediator _mediator;

        public AuctionTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddInMemoryAuctionDb()
                .AddAuctionDbMigrator()
                .AddApplication()
                .BuildServiceProvider();

            _mediator = serviceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Auction_Create_IdShouldBeGreaterThanZero()
        {
            var lot = new Lot("Some description");
            var auction = await _mediator.Send(new CreateAuctionCommand(lot));
            auction.Id.Should().BeGreaterThan(0);
        }
    }
}
