namespace Core.Domains.AuctionAggregate.Events
{
    public static class AuctionDomainEvents
    {
        public static IAuctionDomainEvent Activated() => new Activated();
        public static IAuctionDomainEvent NewBid() => new NewBid();
        public static IAuctionDomainEvent Buyouted() => new Buyouted();
        public static IAuctionDomainEvent Closed() => new Closed();
    }

    public class Activated : IAuctionDomainEvent { }

    public class NewBid : IAuctionDomainEvent { }

    public class Buyouted : IAuctionDomainEvent { }

    public class Closed : IAuctionDomainEvent { }
}
