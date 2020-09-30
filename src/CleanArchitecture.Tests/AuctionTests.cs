using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using MediatR;
using Core.Domains.AuctionAggregate;
using Core.Domains.AuctionAggregate.Results;
using Data.Extensions;
using Infrastructure.Extensions;
using Application.Extensions;
using Application.Mediators.AuctionHandlers.Commands;
using Application.Mediators.AuctionHandlers.Queries;

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
        public async Task Auction_Flow1_ShouldBeSuccess()
        {
            var lot = new Lot(Guid.NewGuid(), "Some description");
            var createAuctionCommand = new CreateAuctionCommand(lot);
            var auction = await _mediator.Send(createAuctionCommand);
            auction.Id.Should().BeGreaterThan(0);

            var (biddingStartsWith, bidStep, buyoutPrice) = (50000, 1000, 1000000);
            var activateAuctionCommand = new ActivateAuctionCommand(auction.Id, DateTimeOffset.Now.AddDays(1), biddingStartsWith, bidStep, buyoutPrice);
            auction = await _mediator.Send(activateAuctionCommand);
            auction.Status.Should().Be(Status.Active);

            var makeBidCommand1 = new MakeBidCommand(auction.Id, new Bid(Guid.NewGuid(), 49000, "Test comment"));
            var bidResult1 = await _mediator.Send(makeBidCommand1);
            bidResult1.Should().BeAssignableTo<BidTooSmall>().Which.MinimumAllowedBidPrice.Should().Be(biddingStartsWith);

            var makeBidCommand2 = new MakeBidCommand(auction.Id, new Bid(Guid.NewGuid(), 65000, "Test comment"));
            var bidResult2 = await _mediator.Send(makeBidCommand2);
            bidResult2.Should().BeOfType<Ok>();

            var closeAuctionCommand = new CloseAuctionCommand(auction.Id);
            auction = await _mediator.Send(closeAuctionCommand);
            auction.Status.Should().Be(Status.Closed);
        }

        [Fact]
        public async Task Auction_Flow2_ShouldBeSuccess()
        {
            var lot = new Lot(Guid.NewGuid(), "Some description");
            var createAuctionCommand = new CreateAuctionCommand(lot);
            var auction = await _mediator.Send(createAuctionCommand);
            auction.Id.Should().BeGreaterThan(0);

            var (biddingStartsWith, bidStep, buyoutPrice) = (50000, 1000, 1000000);
            var activateAuctionCommand = new ActivateAuctionCommand(auction.Id, DateTimeOffset.Now.AddDays(1), biddingStartsWith, bidStep, buyoutPrice);
            auction = await _mediator.Send(activateAuctionCommand);
            auction.Status.Should().Be(Status.Active);

            var makeBidCommand = new MakeBidCommand(auction.Id, new Bid(Guid.NewGuid(), 1000000, "Test comment"));
            var bidResult = await _mediator.Send(makeBidCommand);
            bidResult.Should().BeOfType<Buyout>();

            var activeAuctionIds = await _mediator.Send(new ActiveAuctionIdsQuery());
            activeAuctionIds.Should().NotContain(auction.Id);
        }
    }
}
