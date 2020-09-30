using System;

namespace Core.Domains.AuctionAggregate
{
    public class Lot
    {
        public int Id { get; private set; }
        public Guid RegionId { get; private set; }
        public string Description { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Lot() { }

        public Lot(Guid regionId, string description)
        {
            RegionId = regionId != default
                ? regionId
                : throw new ArgumentOutOfRangeException(nameof(regionId));

            Description = !string.IsNullOrWhiteSpace(description)
                ? description
                : throw new ArgumentOutOfRangeException(nameof(description));

            CreatedAt = DateTimeOffset.Now;
        }
    }
}
