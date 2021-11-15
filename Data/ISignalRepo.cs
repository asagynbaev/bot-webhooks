using System.Threading.Tasks;
using bot_webhooks.Models;

namespace bot_webhooks.Data
{
    public interface ISignalRepo
    {
        Signal GetSignal(string symbol, int direction);
        Task<int> InsertSignal(Signal signal);
        
    }
}