using System;
using MySqlConnector;

namespace bot_webhooks.Models
{
    public class WebHookContext : IDisposable
    {
        public MySqlConnection Connection { get; }

        public WebHookContext(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public void Dispose() => Connection.Dispose();
    }
}