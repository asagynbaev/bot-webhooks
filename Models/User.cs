using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using bot_webhooks.Helpers;
using Microsoft.EntityFrameworkCore;

namespace bot_webhooks.Models
{
    [Table("users")]
    public class User
    {
        internal WebHookContext Db { get; set; }
         public User()
        {
        }

        public User(WebHookContext db)
        {
            Db = db;
        }

        [Key]
        public int ID { get; private set; }
        public string FullName { get; private set; }
        public string ApiKey {get; private set; }
        public string Secret { get; private set; }
        public int IsActive { get; private set; }
        public int Spot { get; private set; }
        public int Futures { get; private set; }

        // TODO: Написать тело метода со всеми сообщениями в телеграм и логи
        public async Task<User> GetUser(int ID) => await Db.Users.Where(x => x.ID == ID).FirstOrDefaultAsync();

        // TODO: Написать тело метода со всеми сообщениями в телеграм и логи
        public async Task<List<User>> GetAllusers()
        {
            try
            {
                  return await Db.Users.ToListAsync();
            }
            catch (System.Exception ex)
            {
                
                TelegramMessenger.SendMessage($"Error: {ex.Message}");
                //_logger.LogError(ex.Message);
                return null;
            }
        }

        // TODO: Написать метод UpdateUserInfo
    }

    
}