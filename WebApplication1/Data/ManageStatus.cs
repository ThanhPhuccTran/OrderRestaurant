using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Settings")]
    public partial class ManageStatus
    {
        [Key]
        public int StatusId { get; set; }
        public int Code { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        
        public List<Order> Orders { get; set; }
        
        public List<Table> Tables { get; set; }
       /* public List<Cart> Carts { get; set; }*/

    }
}
