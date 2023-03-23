using bot_webhooks.Models;
using Microsoft.EntityFrameworkCore;


namespace bot_webhooks.Data
{
    public class WebHookContext : DbContext
    {
        public WebHookContext(DbContextOptions<WebHookContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Signal> Signals { get; set; }
        public DbSet<BinanceFutureAccountInfo> BinanceFutureAccountInfos { get; set; }
    }
}