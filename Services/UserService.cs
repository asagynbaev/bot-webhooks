using System.Collections.Generic;
using System.Linq;
using bot_webhooks.Data;
using bot_webhooks.Models;

namespace bot_webhooks.Services
{
    public class UserService : IUserRepo
    {
        private readonly WebHookContext _db;

        public UserService(WebHookContext db) => (_db) = (db);

        public User AddUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<User> GetAllUsers() => _db.Users.ToList();
        public User GetUser(int ID) => _db.Users.FirstOrDefault(x => x.ID == ID);

        public User UpdateUser(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}