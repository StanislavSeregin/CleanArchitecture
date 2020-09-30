using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using MediatR;
using Core.Domains.AuctionAggregate;
using Data.Extensions;
using Application.Extensions;
using Application.Mediators.AuctionHandlers.Commands;
using Infrastructure.Extensions;
using Core.Domains.AuctionAggregate.Results;

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
                .AddInfrastructure()
                .AddApplication()
                .BuildServiceProvider();

            _mediator = serviceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Auction_Flow_ShouldBeSuccess()
        {
            var lot = new Lot(Guid.NewGuid(), "Some description");
            var createAuctionCommand = new CreateAuctionCommand(lot);
            var auction = await _mediator.Send(createAuctionCommand);
            auction.Id.Should().BeGreaterThan(0);

            var activateAuctionCommand = new ActivateAuctionCommand(auction.Id, DateTimeOffset.Now.AddDays(1));
            auction = await _mediator.Send(activateAuctionCommand);
            auction.Status.Should().Be(Status.Active);

            var makeBidCommand = new MakeBidCommand(auction.Id, new Bid(Guid.NewGuid(), 50000, "Test comment"));
            var bidResult = await _mediator.Send(makeBidCommand);
            bidResult.Should().BeOfType<Ok>();
        }
    }
}
