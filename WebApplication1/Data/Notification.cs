using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public bool IsCheck { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
