using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace bot_webhooks.Models
{
    [Table("pairs")]
    public class Signal
    {
        private readonly ILogger<Signal> _logger;
        internal WebHookContext Db { get; set; }

        public Signal()
        {
        }
        internal Signal(WebHookContext db, ILogger<Signal> logger)
        {
            _logger = logger;
            Db = db;
        }
        
        [Column("ID")]
        [Key]
        public int ID { get; protected set; }
        public string Symbol { get; private set; }
        public int BuySignalLevel1 { get; private set; }
        public int BuySignalLevel2 { get; private set; }
        public int BuySignalLevel3 { get; private set; }
        public int SellSignalLevel1 { get; private set; }
        public int SellSignalLevel2 { get; private set; }
        public int SellSignalLevel3 { get; private set; }

        public async Task<Signal> GetDataFromDBAsync(string symbol)
        {
            try
            {
                 return await Db.Signals.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();  
            }
            catch (System.Exception ex)
            {
                TelegramMessenger.SendMessage($"Error: {ex.Message}");
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateDB(string symbol)
        {
            try
            {
                Signal statement = await Db.Signals.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();
                // FIXME: Use only one variable to measure level of signal
                statement.BuySignalLevel1 = 0;
                statement.BuySignalLevel2 = 0;
                statement.BuySignalLevel3 = 0;
                statement.SellSignalLevel1 = 0;
                statement.SellSignalLevel2 = 0;
                statement.SellSignalLevel3 = 0;

                await Db.SaveChangesAsync();
                return true;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                TelegramMessenger.SendMessage($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDB(string symbol, int signal)
        {
            try
            {
                Signal statement = await Db.Signals.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();

                switch (signal)
                {
                    case 1:
                        statement.BuySignalLevel1 = 1;
                        break;
                    case 2:
                        statement.BuySignalLevel2 = 1;
                        break;
                    case 3:
                        statement.BuySignalLevel3 = 1;
                        break;
                    case 4:
                        statement.SellSignalLevel1 = 1;
                        break;
                    case 5:
                        statement.SellSignalLevel2 = 1;
                        break;
                    default:
                        statement.SellSignalLevel3 = 1;
                        break;
                }

                await Db.SaveChangesAsync();
                return true;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                TelegramMessenger.SendMessage($"Error: {ex.Message}");
                return false;
            }
        }
    }
}