using bot_webhooks.Data;
using bot_webhooks.Models;

namespace bot_webhooks.Services;

public class SignalService(WebHookContext db) : ISignalRepo
{
	private readonly WebHookContext _db = db;

	public Signal? GetSignal(string symbol, int direction) =>
		_db.Signals
			.FirstOrDefault(x => x.Symbol == symbol && x.Direction == direction && x.IsActive == 1);

	public async Task<int> InsertSignal(Signal signal)
	{
		await _db.AddAsync(signal);
		await _db.SaveChangesAsync();
		return signal.ID;
	}
}
