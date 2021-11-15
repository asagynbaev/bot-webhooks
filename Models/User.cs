using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace bot_webhooks.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int ID { get; private set; }
        public string FullName { get; private set; }
        public string ApiKey {get; private set; }
        public string Secret { get; private set; }
        public int IsActive { get; private set; }
        public int Spot { get; private set; }
        public int Futures { get; private set; }
    }
}