namespace Core.Domain.Auction.Results
{
    public static class BidResults
    {
        public static IBidResult Ok() => new Ok();
        public static IBidResult Buyout() => new Buyout();
        public static IBidResult BidTooSmall(decimal minimumAllowedBidPrice) => new BidTooSmall(minimumAllowedBidPrice);
    }

    public class Ok : IBidResult { }

    public class Buyout : IBidResult { }

    public class BidTooSmall : IBidResult
    {
        public decimal MinimumAllowedBidPrice { get; private set; }

        public BidTooSmall(decimal minimumAllowedBidPrice)
        {
            MinimumAllowedBidPrice = minimumAllowedBidPrice;
        }
    }
}
