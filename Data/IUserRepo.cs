using System.Collections.Generic;
using bot_webhooks.Models;

namespace bot_webhooks.Data
{
    public interface IUserRepo
    {
        User AddUser(User user);
        User GetUser(int ID);
        IEnumerable<User> GetAllUsers();
        User UpdateUser(User user);
    }
}