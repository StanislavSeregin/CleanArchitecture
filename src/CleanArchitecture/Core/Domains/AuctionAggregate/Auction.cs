using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domains.AuctionAggregate.Events;
using Core.Domains.AuctionAggregate.Results;

namespace Core.Domains.AuctionAggregate
{
    public class Auction
    {
        public const decimal DefaultBiddingStartsWith = 1000;
        public const decimal DefaultBidStep = 1000;

        public int Id { get; private set; }
        public Lot Lot { get; private set; }
        public Status Status { get; private set; }
        public decimal BiddingStartsWith { get; private set; }
        public decimal BidStep { get; private set; }
        public decimal? BuyoutPrice { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ActivatedAt { get; private set; }
        public DateTimeOffset? CloseTo { get; private set; }
        public DateTimeOffset? ClosedAt { get; private set; }

        private readonly List<Bid> _bids;
        public IReadOnlyCollection<Bid> Bids => _bids;

        private List<IAuctionDomainEvent> DomainEvents { get; set; }

        private Auction() { }

        public Auction(Lot lot)
        {
            Lot = lot ?? throw new ArgumentNullException(nameof(lot));
            Status = Status.None;
            BiddingStartsWith = DefaultBiddingStartsWith;
            BidStep = DefaultBidStep;
            CreatedAt = DateTimeOffset.Now;
            _bids = new List<Bid>();
        }

        public Auction Activate(DateTimeOffset closeTo)
        {
            if (Status != Status.None)
                throw new InvalidOperationException($"{nameof(Status)} should be {nameof(Status.None)}");

            var now = DateTimeOffset.Now;
            CloseTo = closeTo > now
                ? closeTo
                : throw new ArgumentOutOfRangeException(nameof(closeTo));

            ActivatedAt = now;
            Status = Status.Active;
            AddDomainEvent(AuctionDomainEvents.Activated());
            return this;
        }

        public Auction ConfigureRules(
            decimal biddingStartsWith,
            decimal bidStep,
            decimal? buyoutPrice = null
        )
        {
            if (Status == Status.Closed)
                throw new InvalidOperationException($"{nameof(Status)} should not be '{nameof(Status.Closed)}'");

            BiddingStartsWith = biddingStartsWith <= 0
                ? throw new ArgumentOutOfRangeException(nameof(biddingStartsWith))
                : biddingStartsWith;

            BidStep = bidStep <= 0
                ? throw new ArgumentOutOfRangeException(nameof(bidStep))
                : bidStep;

            BuyoutPrice = buyoutPrice switch
            {
                null => null,
                _ when buyoutPrice < GetMinimumAllowedBidPrice() => throw new ArgumentOutOfRangeException(nameof(buyoutPrice)),
                _ => buyoutPrice
            };

            return this;
        }

        public IBidResult MakeBid(Bid bid)
        {
            if (Status != Status.Active)
                throw new InvalidOperationException($"{nameof(Status)} should be {nameof(Status.Active)}");

            if (bid == null)
                throw new ArgumentNullException(nameof(bid));

            var minimumAllowedBidPrice = GetMinimumAllowedBidPrice();
            if (bid.Price < minimumAllowedBidPrice)
                return BidResults.BidTooSmall(minimumAllowedBidPrice);

            _bids.Add(bid);
            if (BuyoutPrice.HasValue && bid.Price > BuyoutPrice)
            {
                Close();
                AddDomainEvent(AuctionDomainEvents.Buyouted());
                return BidResults.Buyout();
            }
            else
            {
                AddDomainEvent(AuctionDomainEvents.NewBid());
                return BidResults.Ok();
            }
        }

        public Auction Close()
        {
            if (Status == Status.Closed)
                throw new InvalidOperationException($"{nameof(Status)} already '{nameof(Status.Closed)}'");

            Status = Status.Closed;
            ClosedAt = DateTimeOffset.Now;
            AddDomainEvent(AuctionDomainEvents.Closed());
            return this;
        }

        public IEnumerable<IAuctionDomainEvent> GetDomainEvents()
            => DomainEvents.AsEnumerable() ?? Array.Empty<IAuctionDomainEvent>();

        private void AddDomainEvent(IAuctionDomainEvent domainEvent)
        {
            DomainEvents ??= new List<IAuctionDomainEvent>();
            DomainEvents.Add(domainEvent);
        }

        private decimal GetMinimumAllowedBidPrice() => GetActualBidPrice() switch
        {
            0 => BiddingStartsWith,
            var actualBidPrice => Math.Max(actualBidPrice + BidStep, BiddingStartsWith)
        };

        private decimal GetActualBidPrice()
            => _bids
                .Select(bid => bid.Price)
                .DefaultIfEmpty(0)
                .Max();
    }
}
