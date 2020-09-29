using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Auction.Events;
using Core.Domain.Auction.Results;

namespace Core.Domain.Auction
{
    public class Auction
    {
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

        private List<IDomainEvent> DomainEvents { get; set; }

        private Auction() { }

        public Auction(Lot lot)
        {
            Lot = lot ?? throw new ArgumentNullException(nameof(lot));
            Status = Status.None;
            BiddingStartsWith = 1000;
            BidStep = 1000;
            CreatedAt = DateTimeOffset.Now;
            _bids = new List<Bid>();
            AddDomainEvent(Events.DomainEvents.Created());
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
            AddDomainEvent(Events.DomainEvents.Activated());
            return this;
        }

        public Auction Close()
        {
            if (Status == Status.Closed)
                throw new InvalidOperationException($"{nameof(Status)} already '{nameof(Status.Closed)}'");

            Status = Status.Closed;
            ClosedAt = DateTimeOffset.Now;
            AddDomainEvent(Events.DomainEvents.Closed());
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
                AddDomainEvent(Events.DomainEvents.Buyouted());
                return BidResults.Buyout();
            }
            else
            {
                AddDomainEvent(Events.DomainEvents.NewBid());
                return BidResults.Ok();
            }
        }

        public IEnumerable<IDomainEvent> GetDomainEvents()
            => DomainEvents.AsEnumerable() ?? Array.Empty<IDomainEvent>();

        private void AddDomainEvent(IDomainEvent domainEvent)
        {
            DomainEvents ??= new List<IDomainEvent>();
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
