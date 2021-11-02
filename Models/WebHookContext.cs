using Microsoft.EntityFrameworkCore;


namespace bot_webhooks.Models
{
    public class WebHookContext : DbContext
    {
        public DbSet<Signal> Signals { get; set; }
        public DbSet<User> Users { get; set; }   
        public WebHookContext(DbContextOptions<WebHookContext> options) : base(options)  
        {   
        }  
    }
}