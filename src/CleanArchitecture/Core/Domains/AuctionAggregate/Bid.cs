using System;

namespace Core.Domains.AuctionAggregate
{
    public class Bid
    {
        public int Id { get; private set; }
        public string AccountId { get; private set; }
        public decimal Price { get; private set; }
        public string Comment { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Bid() { }

        public Bid(string accountId, decimal price, string comment = null)
        {
            AccountId = !string.IsNullOrWhiteSpace(accountId)
                ? accountId
                : throw new ArgumentOutOfRangeException(nameof(accountId));

            Price = price > 0
                ? price
                : throw new ArgumentOutOfRangeException(nameof(price));

            Comment = comment;
            CreatedAt = DateTimeOffset.Now;
        }
    }
}
