using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Table")]
    public partial class Table
    {
        [Key]
        public int TableId { get; set; }
        public string TableName { get; set; }
        
        public int StatusId {  get; set; }
        public string? Note { get; set; }
        public string? QR_id { get; set; }
        [ForeignKey("StatusId")]
        public ManageStatus? Statuss { get; set; }
        public List<Order> Orders { get; set; }
        public List<Cart> Carts { get; set; }

       
    }
}
