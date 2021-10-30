using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace bot_webhooks.Models
{
    [Table("pairs")]
    public class Statement
    {
        internal WebHookContext Db { get; set; }

        public Statement()
        {
        }
        internal Statement(WebHookContext db)
        {
            Db = db;
        }
        
        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Symbol { get; set; }
        public int BuySignalLevel1 { get; set; }
        public int BuySignalLevel2 { get; set; }
        public int BuySignalLevel3 { get; set; }
        public int SellSignalLevel1 { get; set; }
        public int SellSignalLevel2 { get; set; }
        public int SellSignalLevel3 { get; set; }
        public int Long { get; set; }
        public int Short { get; set; }

        public async Task<Statement> GetDataFromDBAsync(string symbol)
        {
            return await Db.Statements.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateDB(string symbol)
        {
            try
            {
                Statement statement =  await Db.Statements.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();
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
            catch (System.Exception)
            {
                // TODO: Add to log
                return false;
            }
        }

        public async Task<bool> UpdateDB(string symbol, int signal)
        {
            try
            {
                 Statement statement =  await Db.Statements.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();

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
            catch (System.Exception)
            {
                // TODO: Add to log
                return false;
            }
        }
    }
}