using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace bot_webhooks.Models
{
    public class Position
    {
        public string Symbol { get; set; }
        public string PositionSide { get; set; }
        public decimal EntryPrice { get; set; }

        internal WebHookContext Db { get; set; }

        public Position()
        {
        }

        internal Position(WebHookContext db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `MainTable` (`Symbol`, `PositionSide`, `Price`) VALUES (@Symbol, @PositionSide, @EntryPrice);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@Symbol",
                DbType = DbType.String,
                Value = Symbol,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@PositionSide",
                DbType = DbType.String,
                Value = PositionSide,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@EntryPrice",
                DbType = DbType.Decimal,
                Value = EntryPrice,
            });
        }
    }
}