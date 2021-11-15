using System.Linq;
using System.Threading.Tasks;
using bot_webhooks.Data;
using bot_webhooks.Models;

namespace bot_webhooks.Services
{
    public class SignalService : ISignalRepo
    {
        private readonly WebHookContext _db;

        public SignalService(WebHookContext db) => (_db) = (db);
        
        public Signal GetSignal(string symbol, int direction) => 
            _db.Signals.Where(x => x.Symbol == symbol)
            .Where(x => x.Direction == direction)
            .Where(x=> x.IsActive == 1)
            .FirstOrDefault();

        public async Task<int> InsertSignal(Signal signal)
        {
            await _db.AddAsync(signal);
            await _db.SaveChangesAsync();
            int id = signal.ID;
            return id;
        }
    }
}