using System;

namespace Core.Domains.AuctionAggregate
{
    public class Bid
    {
        public int Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public decimal Price { get; private set; }
        public string Comment { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Bid() { }

        public Bid(Guid customerId, decimal price, string comment = null)
        {
            CustomerId = customerId != default
                ? customerId
                : throw new ArgumentOutOfRangeException(nameof(customerId));

            Price = price > 0
                ? price
                : throw new ArgumentOutOfRangeException(nameof(price));

            Comment = comment;
            CreatedAt = DateTimeOffset.Now;
        }
    }
}
