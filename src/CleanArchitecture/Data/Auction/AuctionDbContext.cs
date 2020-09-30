using Microsoft.EntityFrameworkCore;

namespace Data.Auction
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Core.Domains.AuctionAggregate.Auction> Auctions { get; set; }
    }
}
