using System.Threading.Tasks;

namespace bot_webhooks.Models
{
    public abstract class Position
    {
        public Signal signal { get; set; }

        public abstract Task<bool> OpenPosition(Signal signal);
        public abstract Task<bool> ClosePosition(Signal signal);
        public abstract void GetBalance(string apiKey, string secret);
        public abstract void GetCurrentPosition(string symbol, string apiKey, string secret);
    }
}