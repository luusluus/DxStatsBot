using DXStats.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace DXStats.Persistence
{
    public class DxStatsDbContext : DbContext
    {
        public DxStatsDbContext(DbContextOptions<DxStatsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Trade> Trades { get; set; }
        public DbSet<Coin> Coins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trade>().HasKey(t => t.Id);
            modelBuilder.Entity<Coin>().HasKey(c => c.Id);
        }
    }
}
