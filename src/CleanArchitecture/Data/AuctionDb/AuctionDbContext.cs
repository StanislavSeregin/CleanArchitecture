using Microsoft.EntityFrameworkCore;
using Core.Domains.AuctionAggregate;

namespace Data.AuctionDb
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
    }
}
