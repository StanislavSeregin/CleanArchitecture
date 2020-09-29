namespace Core.Domains.AuctionAggregate.Events
{
    public static class DomainEvents
    {
        public static IDomainEvent Created() => new Created();
        public static IDomainEvent Activated() => new Activated();
        public static IDomainEvent NewBid() => new NewBid();
        public static IDomainEvent Buyouted() => new Buyouted();
        public static IDomainEvent Closed() => new Closed();
    }

    public class Created : IDomainEvent { }

    public class Activated : IDomainEvent { }

    public class NewBid : IDomainEvent { }

    public class Buyouted : IDomainEvent { }

    public class Closed : IDomainEvent { }
}
