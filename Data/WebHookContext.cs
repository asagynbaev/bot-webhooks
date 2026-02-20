using bot_webhooks.Models;
using Microsoft.EntityFrameworkCore;

namespace bot_webhooks.Data;

public class WebHookContext(DbContextOptions<WebHookContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<Trade> Trades { get; set; }
	public DbSet<Signal> Signals { get; set; }
	public DbSet<BinanceFutureAccountInfo> BinanceFutureAccountInfos { get; set; }
}
