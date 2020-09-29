using System;

namespace Core.Domain.Auction
{
    public class Lot
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Lot() { }

        public Lot(string description)
        {
            Description = !string.IsNullOrWhiteSpace(description)
                ? description
                : throw new InvalidOperationException($"{nameof(description)} should not be empty or null");

            CreatedAt = DateTimeOffset.Now;
        }
    }
}
