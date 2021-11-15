using bot_webhooks.Models;
using Microsoft.EntityFrameworkCore;


namespace bot_webhooks.Data
{
    public class WebHookContext : DbContext
    {
        public WebHookContext(DbContextOptions<WebHookContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Signal> Signals { get; set; }
    }
}