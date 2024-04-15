using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Table")]
    public class Table
    {
        [Key]
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int Status {  get; set; }
        public string Note { get; set; }
    }
}
