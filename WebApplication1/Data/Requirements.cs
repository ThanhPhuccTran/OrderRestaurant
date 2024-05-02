using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Requirements")]
    public class Requirements
    {
        [Key]
        public int RequestId { get; set; }
        public int TableId { get; set; }
        public DateTime? RequestTime { get; set; }
        public string Title { get; set; }
        public string? RequestNote { get; set; }
        public int Code { get; set; }
        [ForeignKey("TableId")]
        public Table Tables { get; set; }
    }
}
