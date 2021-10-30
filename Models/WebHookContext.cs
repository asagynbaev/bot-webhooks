using Microsoft.EntityFrameworkCore;


namespace bot_webhooks.Models
{
    public class WebHookContext : DbContext
    {
        public DbSet<Statement> Statements { get; set; }    
        public WebHookContext(DbContextOptions<WebHookContext> options) : base(options)  
        {   
        }  
    }
}